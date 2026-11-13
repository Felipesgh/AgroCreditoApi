using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AgroCredito.Application.DTOs;
using AgroCredito.Domain.Enums;
using Xunit;

namespace AgroCredito.Api.Tests.Integration;

[Collection("Integracao")]
public class PropriedadesESimulacoesIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private static async Task<PropriedadeResponse> CriarAsync(HttpClient cliente, string nome = "Sitio Boa Vista")
    {
        var resposta = await cliente.PostAsJsonAsync("/api/propriedades", ApiTestHelpers.PropriedadeValida(nome));
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<PropriedadeResponse>())!;
    }

    [Fact]
    public async Task Propriedade_CicloCompleto_CriarBuscarAtualizarExcluir()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);

        var criada = await cliente.PostAsJsonAsync("/api/propriedades", ApiTestHelpers.PropriedadeValida());
        Assert.Equal(HttpStatusCode.Created, criada.StatusCode);
        Assert.NotNull(criada.Headers.Location);
        var propriedade = await criada.Content.ReadFromJsonAsync<PropriedadeResponse>();

        var busca = await cliente.GetAsync($"/api/propriedades/{propriedade!.Id}");
        Assert.Equal(HttpStatusCode.OK, busca.StatusCode);

        var atualizacao = await cliente.PutAsJsonAsync($"/api/propriedades/{propriedade.Id}", ApiTestHelpers.PropriedadeValida("Sitio Renomeado"));
        Assert.Equal(HttpStatusCode.OK, atualizacao.StatusCode);
        var atualizada = await atualizacao.Content.ReadFromJsonAsync<PropriedadeResponse>();
        Assert.Equal("Sitio Renomeado", atualizada!.Nome);

        var exclusao = await cliente.DeleteAsync($"/api/propriedades/{propriedade.Id}");
        Assert.Equal(HttpStatusCode.NoContent, exclusao.StatusCode);

        var depois = await cliente.GetAsync($"/api/propriedades/{propriedade.Id}");
        Assert.Equal(HttpStatusCode.NotFound, depois.StatusCode);
    }

    [Fact]
    public async Task Propriedade_ComDadosInvalidos_Retorna400ComErros()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var invalida = new PropriedadeRequest("", "X", "", 0m, TipoProducao.Misto, "");

        var resposta = await cliente.PostAsJsonAsync("/api/propriedades", invalida);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        var corpo = (await resposta.Content.ReadAsStringAsync()).ToLowerInvariant();
        Assert.Contains("areahectares", corpo);
        Assert.Contains("estado", corpo);
    }

    [Fact]
    public async Task Propriedade_DeOutroProdutor_Retorna404()
    {
        var dono = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var outro = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var propriedade = await CriarAsync(dono);

        var resposta = await outro.GetAsync($"/api/propriedades/{propriedade.Id}");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Recomendacoes_RetornamSomenteOportunidadesCompativeis()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var propriedade = await CriarAsync(cliente);

        var recomendacao = await cliente.GetFromJsonAsync<RecomendacaoResponse>($"/api/propriedades/{propriedade.Id}/recomendacoes");

        var ids = recomendacao!.Oportunidades.Select(o => o.Id).OrderBy(id => id).ToArray();
        Assert.Equal(new[] { 1, 2, 4, 5 }, ids);
        Assert.False(string.IsNullOrWhiteSpace(recomendacao.Aviso));
    }

    [Fact]
    public async Task Simular_ComDadosValidos_Retorna201EGuardaNoHistorico()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var propriedade = await CriarAsync(cliente);

        var resposta = await cliente.PostAsJsonAsync($"/api/propriedades/{propriedade.Id}/simulacoes", new SimulacaoRequest(1, 50000m, 24));

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var simulacao = await resposta.Content.ReadFromJsonAsync<SimulacaoResponse>();
        Assert.InRange(simulacao!.ValorParcela, 2148m, 2149m);
        Assert.Equal("Pronaf Custeio", simulacao.Oportunidade);
        Assert.True(simulacao.TotalPago > 50000m);

        var historico = await cliente.GetFromJsonAsync<List<SimulacaoResponse>>($"/api/propriedades/{propriedade.Id}/simulacoes");
        Assert.Single(historico!);
    }

    [Fact]
    public async Task Simular_ComPropriedadeNaoElegivel_Retorna422()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var propriedade = await CriarAsync(cliente);

        var resposta = await cliente.PostAsJsonAsync($"/api/propriedades/{propriedade.Id}/simulacoes", new SimulacaoRequest(3, 50000m, 24));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
        using var corpo = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal(422, corpo.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task Simular_ComValorAcimaDoMaximoDaLinha_Retorna422()
    {
        var cliente = await ApiTestHelpers.CriarClienteAutenticadoAsync(factory);
        var propriedade = await CriarAsync(cliente);

        var resposta = await cliente.PostAsJsonAsync($"/api/propriedades/{propriedade.Id}/simulacoes", new SimulacaoRequest(1, 900000m, 24));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, resposta.StatusCode);
    }
}
