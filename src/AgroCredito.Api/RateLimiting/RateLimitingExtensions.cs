using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AgroCredito.Api.RateLimiting;

public static class RateLimitingExtensions
{
    public const string AuthPolicy = "auth";

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, RateLimitingOptions config)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(http =>
            {
                if (EhRotaLivre(http.Request.Path))
                    return RateLimitPartition.GetNoLimiter("livre");

                var produtorId = http.User.FindFirst("sub")?.Value;
                var chave = produtorId is not null ? $"produtor:{produtorId}" : $"ip:{ObterIp(http)}";

                return RateLimitPartition.GetSlidingWindowLimiter(chave, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = config.Global.PermitLimit,
                    Window = TimeSpan.FromSeconds(config.Global.WindowSeconds),
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            options.AddPolicy(AuthPolicy, http =>
                RateLimitPartition.GetFixedWindowLimiter($"ip:{ObterIp(http)}", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = config.Auth.PermitLimit,
                    Window = TimeSpan.FromSeconds(config.Auth.WindowSeconds),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));

            options.OnRejected = async (contexto, ct) =>
            {
                var http = contexto.HttpContext;
                var segundos = config.Global.WindowSeconds;

                if (contexto.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    segundos = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));

                http.Response.Headers.RetryAfter = segundos.ToString();
                http.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                var problemDetails = http.RequestServices.GetRequiredService<IProblemDetailsService>();
                await problemDetails.TryWriteAsync(new ProblemDetailsContext
                {
                    HttpContext = http,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Muitas requisições",
                        Detail = $"Limite de requisições excedido. Tente novamente em {segundos} segundo(s)."
                    }
                });
            };
        });

        return services;
    }

    private static bool EhRotaLivre(PathString caminho)
        => caminho.StartsWithSegments("/swagger") || caminho.StartsWithSegments("/health");

    private static string ObterIp(HttpContext http)
        => http.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";
}
