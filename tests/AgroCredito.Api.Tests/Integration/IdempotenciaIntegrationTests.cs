using System.Net;
using System.Net.Http.Json;
using AgroCredito.Application.DTOs;
using Xunit;

namespace AgroCredito.Api.Tests.Integration;

[Collection("Integracao")]
public class IdempotenciaIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static HttpRequestMessage CriarPedido(string? chave, PropriedadeRequest corpo)
    {
        var pedido = new HttpRequestMessage(HttpMethod.Post, "/api/propriedades") { Content = JsonContent.Create(corpo) };
        if (chave is not null)
            pedido.Headers.Add("Idempotency-Key", chave);
        return pedido;
    }

    [Fact]
    public async Task MesmaChaveEMesmoConteudo_DevolveAMesmaRespostaSemCriarDeNovo()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var chave = Guid.NewGuid().ToString();
        var corpo = ApiTestHelpers.PropriedadeValida("Sitio Idempotente");

        var primeira = await cliente.SendAsync(CriarPedido(chave, corpo));
        var segunda = await cliente.SendAsync(CriarPedido(chave, corpo));

        Assert.Equal(HttpStatusCode.Created, primeira.StatusCode);
        Assert.Equal(HttpStatusCode.Created, segunda.StatusCode);
        Assert.False(primeira.Headers.Contains("Idempotent-Replayed"));
        Assert.True(segunda.Headers.Contains("Idempotent-Replayed"));

        var a = await primeira.Content.ReadFromJsonAsync<PropriedadeResponse>();
        var b = await segunda.Content.ReadFromJsonAsync<PropriedadeResponse>();
        Assert.Equal(a!.Id, b!.Id);

        var lista = await cliente.GetFromJsonAsync<List<PropriedadeResponse>>("/api/propriedades");
        Assert.Single(lista!);
    }

    [Fact]
    public async Task MesmaChaveComConteudoDiferente_Retorna422()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var chave = Guid.NewGuid().ToString();

        var primeira = await cliente.SendAsync(CriarPedido(chave, ApiTestHelpers.PropriedadeValida("Sitio A")));
        var segunda = await cliente.SendAsync(CriarPedido(chave, ApiTestHelpers.PropriedadeValida("Sitio B")));

        Assert.Equal(HttpStatusCode.Created, primeira.StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, segunda.StatusCode);
    }

    [Fact]
    public async Task SemChave_OsPedidosRepetidosCriamDuasPropriedades()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var corpo = ApiTestHelpers.PropriedadeValida("Sitio Sem Chave");

        await cliente.SendAsync(CriarPedido(null, corpo));
        await cliente.SendAsync(CriarPedido(null, corpo));

        var lista = await cliente.GetFromJsonAsync<List<PropriedadeResponse>>("/api/propriedades");
        Assert.Equal(2, lista!.Count);
    }
}
