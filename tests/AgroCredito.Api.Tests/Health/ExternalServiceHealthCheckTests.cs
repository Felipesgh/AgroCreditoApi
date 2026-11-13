using AgroCredito.Api.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace AgroCredito.Api.Tests.Health;

public class ExternalServiceHealthCheckTests
{
    private sealed class StubHandler(Func<HttpResponseMessage> resposta) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(resposta());
    }

    private sealed class StubFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }

    private static ExternalServiceHealthCheck CriarCheck(Func<HttpResponseMessage> resposta) =>
        new(new StubFactory(new StubHandler(resposta)), new ExternalServiceOptions { Name = "Servico teste", Url = "http://teste.local/ok", TimeoutSeconds = 2 });

    [Fact]
    public async Task Servico_QueResponde200_DeveSerHealthy()
    {
        var check = CriarCheck(() => new HttpResponseMessage(System.Net.HttpStatusCode.OK));

        var resultado = await check.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Healthy, resultado.Status);
        Assert.Equal(200, resultado.Data["statusHttp"]);
    }

    [Fact]
    public async Task Servico_QueResponde500_DeveSerDegraded()
    {
        var check = CriarCheck(() => new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError));

        var resultado = await check.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Degraded, resultado.Status);
        Assert.Equal(500, resultado.Data["statusHttp"]);
    }

    [Fact]
    public async Task Servico_ForaDoAr_DeveSerDegradedENaoLancarExcecao()
    {
        var check = CriarCheck(() => throw new HttpRequestException("sem conexao"));

        var resultado = await check.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Degraded, resultado.Status);
    }
}
