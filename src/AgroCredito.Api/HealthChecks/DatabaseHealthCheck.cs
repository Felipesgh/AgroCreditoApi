using System.Diagnostics;
using AgroCredito.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AgroCredito.Api.HealthChecks;

public sealed class DatabaseHealthCheck(AgroCreditoDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var cronometro = Stopwatch.StartNew();

        try
        {
            var conectou = await db.Database.CanConnectAsync(cancellationToken);

            var dados = new Dictionary<string, object>
            {
                ["provedor"] = "Oracle",
                ["tempoMs"] = cronometro.ElapsedMilliseconds
            };

            return conectou
                ? HealthCheckResult.Healthy("Banco de dados acessível.", dados)
                : HealthCheckResult.Unhealthy("Não foi possível conectar ao banco de dados.", data: dados);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao consultar o banco de dados.", ex);
        }
    }
}
