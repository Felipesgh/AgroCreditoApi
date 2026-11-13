using System.Net.Http.Headers;
using System.Net.Http.Json;
using AgroCredito.Application.DTOs;
using AgroCredito.Domain.Enums;

namespace AgroCredito.Api.Tests.Integration;

internal static class ApiTestHelpers
{
    public const string SenhaPadrao = "Senha1234";

    public static string NovoEmail() => $"produtor-{Guid.NewGuid():N}@teste.com";

    public static async Task<HttpClient> CriarClienteAutenticadoAsync(ApiFactory factory)
    {
        var cliente = factory.CreateClient();
        var email = NovoEmail();

        var cadastro = await cliente.PostAsJsonAsync("/api/auth/register", new RegisterRequest("Produtor de Teste", email, SenhaPadrao));
        cadastro.EnsureSuccessStatusCode();

        var login = await cliente.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, SenhaPadrao));
        login.EnsureSuccessStatusCode();

        var token = await login.Content.ReadFromJsonAsync<TokenResponse>();

        cliente.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token!.AccessToken);

        return cliente;
    }

    public static PropriedadeRequest PropriedadeValida(string nome = "Sitio Boa Vista") =>
        new(nome, "SP", "Ibiuna", 15m, TipoProducao.Horticultura, "Alface");
}
