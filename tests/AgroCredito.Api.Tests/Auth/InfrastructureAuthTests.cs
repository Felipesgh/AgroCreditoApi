using System.IdentityModel.Tokens.Jwt;
using AgroCredito.Infrastructure.Auth;
using Xunit;

namespace AgroCredito.Api.Tests.Auth;

public class Pbkdf2PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _sut = new();

    [Fact]
    public void Hash_NaoDeveGuardarASenhaEmTextoPuro()
    {
        var hash = _sut.Hash("Senha1234");

        Assert.DoesNotContain("Senha1234", hash);
        Assert.True(_sut.Verify("Senha1234", hash));
    }

    [Fact]
    public void Hash_DeveGerarValoresDiferentesParaAMesmaSenha()
    {
        Assert.NotEqual(_sut.Hash("Senha1234"), _sut.Hash("Senha1234"));
    }

    [Fact]
    public void Verify_DeveFalhar_ComSenhaErrada()
    {
        var hash = _sut.Hash("Senha1234");

        Assert.False(_sut.Verify("OutraSenha1", hash));
    }

    [Fact]
    public void Verify_DeveFalhar_QuandoOHashEstaMalFormado()
    {
        Assert.False(_sut.Verify("Senha1234", "isto-nao-e-um-hash"));
    }
}

public class JwtTokenServiceTests
{
    private static JwtOptions Opcoes() => new()
    {
        Key = new string('k', 40),
        Issuer = "teste",
        Audience = "teste",
        ExpirationMinutes = 5
    };

    [Fact]
    public void CreateToken_DeveConterOsDadosDoProdutor()
    {
        var sut = new JwtTokenService(Opcoes());

        var token = new JwtSecurityTokenHandler().ReadJwtToken(sut.CreateToken(7, "Maria", "maria@teste.com"));

        Assert.Equal("7", token.Subject);
        Assert.Equal("Maria", token.Claims.First(c => c.Type == "name").Value);
        Assert.Equal("Produtor", token.Claims.First(c => c.Type == "role").Value);
        Assert.Equal("teste", token.Issuer);
        Assert.Contains("teste", token.Audiences);
        Assert.True(token.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public void ExpirationSeconds_DeveSerOPrazoEmSegundos()
    {
        Assert.Equal(300, new JwtTokenService(Opcoes()).ExpirationSeconds);
    }
}
