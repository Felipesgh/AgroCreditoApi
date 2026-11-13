using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AgroCredito.Api.HealthChecks;

public sealed class ExternalServiceHealthCheck(IHttpClientFactory httpClientFactory, ExternalServiceOptions options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var cronometro = Stopwatch.StartNew();

        try
        {
            using var limite = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            limite.CancelAfter(TimeSpan.FromSeconds(options.TimeoutSeconds));

            var cliente = httpClientFactory.CreateClient(nameof(ExternalServiceHealthCheck));
            using var resposta = await cliente.GetAsync(options.Url, HttpCompletionOption.ResponseHeadersRead, limite.Token);

            var dados = new Dictionary<string, object>
            {
                ["servico"] = options.Name,
                ["statusHttp"] = (int)resposta.StatusCode,
                ["tempoMs"] = cronometro.ElapsedMilliseconds
            };

            return resposta.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"{options.Name} respondeu {(int)resposta.StatusCode}.", dados)
                : HealthCheckResult.Degraded($"{options.Name} respondeu {(int)resposta.StatusCode}.", data: dados);
        }
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException)
        {
            return HealthCheckResult.Degraded($"{options.Name} indisponível.", ex);
        }
    }
}
