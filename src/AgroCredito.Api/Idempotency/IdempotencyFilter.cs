using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AgroCredito.Api.Endpoints;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Services;
using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Exceptions;

namespace AgroCredito.Api.Idempotency;

/// <summary>
/// Evita operações duplicadas: se o cliente repetir um POST com a mesma Idempotency-Key,
/// a API devolve a resposta da primeira vez em vez de criar de novo.
/// </summary>
public sealed class IdempotencyFilter(IdempotencyService idempotency) : IEndpointFilter
{
    public const string HeaderName = "Idempotency-Key";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        // A chave é opcional e só vale para POST (criação). Sem ela, o fluxo é o normal.
        if (!HttpMethods.IsPost(http.Request.Method)
            || !http.Request.Headers.TryGetValue(HeaderName, out var valores)
            || string.IsNullOrWhiteSpace(valores.ToString()))
        {
            return await next(context);
        }

        var chave = valores.ToString().Trim();
        var produtorId = http.User.GetProdutorId();
        var hash = CalcularHash(http, context.Arguments);

        var decisao = await idempotency.BeginAsync(produtorId, chave, hash, http.RequestAborted);

        switch (decisao.Outcome)
        {
            case IdempotencyOutcome.Replay:
                return Repetir(http, decisao.Record!);

            case IdempotencyOutcome.Mismatch:
                throw new DomainException("Esta Idempotency-Key já foi usada em uma requisição diferente.");

            case IdempotencyOutcome.InProgress:
                throw new ConflictException("Já existe uma requisição em andamento com esta Idempotency-Key.");
        }

        try
        {
            var resultado = await next(context);
            await SalvarRespostaAsync(produtorId, chave, resultado, http.RequestAborted);
            return resultado;
        }
        catch
        {
            // Falhou (400, 404, 422...): libera a chave para o cliente poder tentar de novo.
            await idempotency.ReleaseAsync(produtorId, chave, CancellationToken.None);
            throw;
        }
    }

    private async Task SalvarRespostaAsync(int produtorId, string chave, object? resultado, CancellationToken ct)
    {
        if (resultado is IStatusCodeHttpResult { StatusCode: { } status })
        {
            var corpo = resultado is IValueHttpResult { Value: { } valor }
                ? JsonSerializer.Serialize(valor, valor.GetType(), JsonOptions)
                : null;

            var local = resultado.GetType().GetProperty("Location")?.GetValue(resultado) as string;

            await idempotency.CompleteAsync(produtorId, chave, status, corpo, local, ct);
        }
        else
        {
            await idempotency.ReleaseAsync(produtorId, chave, ct);
        }
    }

    private static IResult Repetir(HttpContext http, IdempotencyRecord registro)
    {
        http.Response.Headers["Idempotent-Replayed"] = "true";

        if (!string.IsNullOrEmpty(registro.Location))
            http.Response.Headers.Location = registro.Location;

        var status = registro.StatusCode ?? StatusCodes.Status200OK;

        return string.IsNullOrEmpty(registro.ResponseBody)
            ? Results.StatusCode(status)
            : Results.Text(registro.ResponseBody, "application/json", Encoding.UTF8, status);
    }

    private static string CalcularHash(HttpContext http, IList<object?> argumentos)
    {
        // Só o conteúdo enviado pelo cliente (os DTOs) entra no hash; serviços e usuário ficam de fora.
        var espacoDosDtos = typeof(PropriedadeRequest).Namespace;
        var corpo = argumentos
            .Where(a => a is not null && a.GetType().Namespace == espacoDosDtos)
            .ToList();

        var conteudo = $"{http.Request.Method}|{http.Request.Path}|{JsonSerializer.Serialize(corpo, JsonOptions)}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(conteudo)));
    }
}
