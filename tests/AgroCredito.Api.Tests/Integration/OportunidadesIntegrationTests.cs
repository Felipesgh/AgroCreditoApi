using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AgroCredito.Application.DTOs;
using Xunit;

namespace AgroCredito.Api.Tests.Integration;

[Collection("Integracao")]
public class OportunidadesIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Listar_EPublico_ERetornaAsCincoOportunidadesDoSeed()
    {
        var cliente = factory.CreateClient();

        var resposta = await cliente.GetAsync("/api/oportunidades");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        using var documento = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal(5, documento.RootElement.GetArrayLength());
        Assert.Equal(JsonValueKind.String, documento.RootElement[0].GetProperty("tipo").ValueKind);
    }

    [Fact]
    public async Task Listar_ComFiltroDeTipo_RetornaSomenteOTipoPedido()
    {
        var cliente = factory.CreateClient();

        var lista = await cliente.GetFromJsonAsync<List<OportunidadeResponse>>("/api/oportunidades?tipo=Seguro");

        var seguro = Assert.Single(lista!);
        Assert.Equal("Seguro Rural (PSR)", seguro.Nome);
    }

    [Fact]
    public async Task Detalhar_RetornaOsDocumentosExigidos()
    {
        var cliente = factory.CreateClient();

        var oportunidade = await cliente.GetFromJsonAsync<OportunidadeResponse>("/api/oportunidades/1");

        Assert.Equal("Pronaf Custeio", oportunidade!.Nome);
        Assert.Equal(4, oportunidade.Requisitos.Count);
    }

    [Fact]
    public async Task Detalhar_Inexistente_Retorna404()
    {
        var cliente = factory.CreateClient();

        var resposta = await cliente.GetAsync("/api/oportunidades/999");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }
}
