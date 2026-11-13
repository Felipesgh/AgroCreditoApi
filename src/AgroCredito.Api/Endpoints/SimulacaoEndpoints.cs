using System.Security.Claims;
using AgroCredito.Api.Idempotency;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgroCredito.Api.Endpoints;

public static class SimulacaoEndpoints
{
    public static IEndpointRouteBuilder MapSimulacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/propriedades/{propriedadeId}/simulacoes")
            .WithTags("Simulações")
            .RequireAuthorization();

        group.MapPost("/", async (
                int propriedadeId,
                [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
                SimulacaoRequest request,
                ClaimsPrincipal user,
                SimulacaoService service,
                CancellationToken ct) =>
            {
                var simulacao = await service.SimularAsync(propriedadeId, user.GetProdutorId(), request, ct);
                return TypedResults.Created($"/api/propriedades/{propriedadeId}/simulacoes", simulacao);
            })
            .AddEndpointFilter<IdempotencyFilter>()
            .WithName("SimularFinanciamento")
            .WithSummary("Simula as parcelas de um crédito para a propriedade")
            .WithDescription("Estimativa pela tabela Price. 422 quando a propriedade não atende aos critérios ou o valor/prazo excede o limite da linha. Envie o header Idempotency-Key para evitar simulações duplicadas.")
            .Produces<SimulacaoResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/", async (int propriedadeId, ClaimsPrincipal user, SimulacaoService service, CancellationToken ct) =>
                TypedResults.Ok(await service.ListAsync(propriedadeId, user.GetProdutorId(), ct)))
            .WithName("ListarSimulacoes")
            .WithSummary("Histórico de simulações da propriedade")
            .Produces<IReadOnlyList<SimulacaoResponse>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
