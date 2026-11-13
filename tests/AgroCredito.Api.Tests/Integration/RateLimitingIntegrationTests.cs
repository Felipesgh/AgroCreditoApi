using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AgroCredito.Application.DTOs;
using Xunit;

namespace AgroCredito.Api.Tests.Integration;

[Collection("Integracao")]
public sealed class RateLimitingIntegrationTests : IDisposable
{
    // Cada teste sobe a sua própria API, com limites baixos para o 429 aparecer rápido.
    private readonly ApiFactory _factory = new LimitedApiFactory();

    public void Dispose() => _factory.Dispose();

    private sealed class LimitedApiFactory : ApiFactory
    {
        public LimitedApiFactory() : base(new Dictionary<string, string>
        {
            ["RateLimiting__Auth__PermitLimit"] = "3",
            ["RateLimiting__Global__PermitLimit"] = "5"
        })
        {
        }
    }

    [Fact]
    public async Task Login_AcimaDoLimite_Retorna429ComRetryAfter()
    {
        var cliente = _factory.CreateClient();
        var tentativa = new LoginRequest("ninguem@teste.com", "Senha1234");

        for (var i = 0; i < 3; i++)
        {
            var dentroDoLimite = await cliente.PostAsJsonAsync("/api/auth/login", tentativa);
            Assert.Equal(HttpStatusCode.Unauthorized, dentroDoLimite.StatusCode);
        }

        var bloqueada = await cliente.PostAsJsonAsync("/api/auth/login", tentativa);

        Assert.Equal(HttpStatusCode.TooManyRequests, bloqueada.StatusCode);
        Assert.True(bloqueada.Headers.Contains("Retry-After"));
        using var corpo = JsonDocument.Parse(await bloqueada.Content.ReadAsStringAsync());
        Assert.Equal(429, corpo.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task LimiteGeral_AcimaDoLimite_Retorna429()
    {
        var cliente = _factory.CreateClient();

        for (var i = 0; i < 5; i++)
        {
            var dentroDoLimite = await cliente.GetAsync("/api/oportunidades");
            Assert.Equal(HttpStatusCode.OK, dentroDoLimite.StatusCode);
        }

        var bloqueada = await cliente.GetAsync("/api/oportunidades");

        Assert.Equal(HttpStatusCode.TooManyRequests, bloqueada.StatusCode);
    }
}
