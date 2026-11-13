using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AgroCredito.Api.HealthChecks;

public static class HealthResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public static Task WriteAsync(HttpContext context, HealthReport relatorio)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var resposta = new
        {
            status = relatorio.Status.ToString(),
            duracaoMs = (long)relatorio.TotalDuration.TotalMilliseconds,
            verificacoes = relatorio.Entries.Select(e => new
            {
                nome = e.Key,
                status = e.Value.Status.ToString(),
                descricao = e.Value.Description,
                duracaoMs = (long)e.Value.Duration.TotalMilliseconds,
                dados = e.Value.Data
            })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(resposta, JsonOptions));
    }
}
