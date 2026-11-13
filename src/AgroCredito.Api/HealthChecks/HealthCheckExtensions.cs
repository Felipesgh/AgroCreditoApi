using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AgroCredito.Api.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services, ExternalServiceOptions externo)
    {
        services.AddHttpClient(nameof(ExternalServiceHealthCheck), cliente =>
            cliente.DefaultRequestHeaders.UserAgent.ParseAdd("AgroCreditoApi-HealthCheck/1.0"));

        services.AddHealthChecks()
            .AddCheck("api", () => HealthCheckResult.Healthy("API no ar."), tags: new[] { "live" })
            .AddCheck<DatabaseHealthCheck>("banco-oracle", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready" })
            .AddTypeActivatedCheck<ExternalServiceHealthCheck>(
                "servico-externo",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "ready", "external" },
                args: new object[] { externo });

        return services;
    }

    public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = verificacao => verificacao.Tags.Contains("live"),
            ResponseWriter = HealthResponseWriter.WriteAsync
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = verificacao => verificacao.Tags.Contains("ready"),
            ResponseWriter = HealthResponseWriter.WriteAsync
        });

        return app;
    }
}
