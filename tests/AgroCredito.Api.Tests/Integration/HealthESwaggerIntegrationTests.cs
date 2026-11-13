using System.Net;
using System.Text.Json;
using Xunit;

namespace AgroCredito.Api.Tests.Integration;

[Collection("Integracao")]
public class HealthESwaggerIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Health_RetornaHealthy()
    {
        var cliente = factory.CreateClient();

        var resposta = await cliente.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        using var documento = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal("Healthy", documento.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task HealthReady_MostraBancoSaudavelEServicoExternoDegradado()
    {
        var cliente = factory.CreateClient();

        var resposta = await cliente.GetAsync("/health/ready");

        // Degraded (serviço externo fora) ainda responde 200: a API continua funcionando.
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        using var documento = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal("Degraded", documento.RootElement.GetProperty("status").GetString());

        var status = documento.RootElement.GetProperty("verificacoes").EnumerateArray()
            .ToDictionary(v => v.GetProperty("nome").GetString()!, v => v.GetProperty("status").GetString());

        Assert.Equal("Healthy", status["banco-oracle"]);
        Assert.Equal("Degraded", status["servico-externo"]);
    }

    [Fact]
    public async Task Swagger_DescreveAsRotasEOEsquemaBearer()
    {
        var cliente = factory.CreateClient();

        var resposta = await cliente.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        using var documento = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        var paths = documento.RootElement.GetProperty("paths");

        Assert.True(paths.TryGetProperty("/api/propriedades", out _));
        Assert.True(paths.TryGetProperty("/api/oportunidades", out _));
        Assert.True(documento.RootElement.GetProperty("components").GetProperty("securitySchemes").TryGetProperty("Bearer", out _));
    }

    [Fact]
    public async Task Swagger_MarcaComCadeadoSomenteAsRotasProtegidas()
    {
        var cliente = factory.CreateClient();

        var resposta = await cliente.GetAsync("/swagger/v1/swagger.json");
        using var documento = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        var paths = documento.RootElement.GetProperty("paths");

        Assert.NotEmpty(paths.GetProperty("/api/propriedades").GetProperty("get").GetProperty("security").EnumerateArray());
        Assert.False(paths.GetProperty("/api/auth/login").GetProperty("post").TryGetProperty("security", out _));
        Assert.False(paths.GetProperty("/api/oportunidades").GetProperty("get").TryGetProperty("security", out _));
    }
}
