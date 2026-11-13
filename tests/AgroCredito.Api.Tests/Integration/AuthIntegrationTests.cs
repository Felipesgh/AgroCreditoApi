using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AgroCredito.Application.DTOs;
using Xunit;

namespace AgroCredito.Api.Tests.Integration;

[Collection("Integracao")]
public class AuthIntegrationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task Cadastro_Login_EPerfil_DevemFuncionar()
    {
        var cliente = factory.CreateClient();
        var email = ApiTestHelpers.NovoEmail();

        var cadastro = await cliente.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Maria Silva", email, ApiTestHelpers.SenhaPadrao));
        Assert.Equal(HttpStatusCode.Created, cadastro.StatusCode);

        var login = await cliente.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, ApiTestHelpers.SenhaPadrao));
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        var token = await login.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.False(string.IsNullOrWhiteSpace(token!.AccessToken));
        Assert.Equal("Bearer", token.TokenType);

        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        var perfil = await cliente.GetFromJsonAsync<ProdutorResponse>("/api/produtores/me");

        Assert.Equal(email, perfil!.Email);
    }

    [Fact]
    public async Task Cadastro_ComEmailRepetido_Retorna409()
    {
        var cliente = factory.CreateClient();
        var pedido = new RegisterRequest("Maria Silva", ApiTestHelpers.NovoEmail(), ApiTestHelpers.SenhaPadrao);

        var primeiro = await cliente.PostAsJsonAsync("/api/auth/register", pedido);
        var segundo = await cliente.PostAsJsonAsync("/api/auth/register", pedido);

        Assert.Equal(HttpStatusCode.Created, primeiro.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segundo.StatusCode);
    }

    [Fact]
    public async Task Login_ComSenhaErrada_Retorna401()
    {
        var cliente = factory.CreateClient();
        var email = ApiTestHelpers.NovoEmail();
        await cliente.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Maria Silva", email, ApiTestHelpers.SenhaPadrao));

        var login = await cliente.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "SenhaErrada99"));

        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }

    [Fact]
    public async Task Cadastro_ComDadosInvalidos_Retorna400ComListaDeErros()
    {
        var cliente = factory.CreateClient();

        var resposta = await cliente.PostAsJsonAsync("/api/auth/register", new RegisterRequest("", "x", "1"));

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        var corpo = (await resposta.Content.ReadAsStringAsync()).ToLowerInvariant();
        Assert.Contains("errors", corpo);
        Assert.Contains("senha", corpo);
    }

    [Fact]
    public async Task RotaProtegida_SemToken_Retorna401()
    {
        var cliente = factory.CreateClient();

        var resposta = await cliente.GetAsync("/api/propriedades");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task RotaProtegida_ComTokenInvalido_Retorna401()
    {
        var cliente = factory.CreateClient();
        cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token-invalido");

        var resposta = await cliente.GetAsync("/api/propriedades");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }
}
