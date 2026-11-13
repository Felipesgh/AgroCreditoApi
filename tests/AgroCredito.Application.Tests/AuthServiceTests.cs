using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Services;
using Xunit;

namespace AgroCredito.Application.Tests;

public class AuthServiceTests
{
    private readonly FakeProdutorRepository _repo = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_repo, new FakePasswordHasher(), new FakeTokenService(), _uow);
    }

    private static RegisterRequest Cadastro() => new("Maria Silva", "  Maria@Teste.com ", "Senha1234");

    [Fact]
    public async Task Register_DeveSalvarComEmailNormalizadoESenhaComHash()
    {
        var response = await _sut.RegisterAsync(Cadastro());

        Assert.Equal("maria@teste.com", response.Email);
        var salvo = Assert.Single(_repo.Items);
        Assert.NotEqual("Senha1234", salvo.SenhaHash);
        Assert.Equal("hash:Senha1234", salvo.SenhaHash);
        Assert.Equal(1, _uow.SaveCount);
    }

    [Fact]
    public async Task Register_DeveFalhar_QuandoEmailJaCadastrado()
    {
        await _sut.RegisterAsync(Cadastro());

        await Assert.ThrowsAsync<ConflictException>(() => _sut.RegisterAsync(Cadastro()));
        Assert.Single(_repo.Items);
    }

    [Fact]
    public async Task Register_DeveFalhar_QuandoDadosInvalidos()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => _sut.RegisterAsync(new RegisterRequest("", "email-invalido", "123")));

        Assert.Contains("Nome", ex.Errors.Keys);
        Assert.Contains("Email", ex.Errors.Keys);
        Assert.Contains("Senha", ex.Errors.Keys);
        Assert.Empty(_repo.Items);
    }

    [Fact]
    public async Task Login_DeveRetornarToken_IgnorandoMaiusculasNoEmail()
    {
        await _sut.RegisterAsync(Cadastro());

        var token = await _sut.LoginAsync(new LoginRequest("MARIA@teste.com", "Senha1234"));

        Assert.Equal("token-1-maria@teste.com", token.AccessToken);
        Assert.Equal("Bearer", token.TokenType);
        Assert.Equal(3600, token.ExpiresIn);
    }

    [Fact]
    public async Task Login_DeveFalhar_QuandoSenhaErrada()
    {
        await _sut.RegisterAsync(Cadastro());

        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _sut.LoginAsync(new LoginRequest("maria@teste.com", "SenhaErrada1")));
    }

    [Fact]
    public async Task Login_DeveFalhar_QuandoEmailNaoExiste()
    {
        await Assert.ThrowsAsync<InvalidCredentialsException>(
            () => _sut.LoginAsync(new LoginRequest("ninguem@teste.com", "Senha1234")));
    }

    [Fact]
    public async Task ObterPerfil_DeveRetornarOProdutor()
    {
        var criado = await _sut.RegisterAsync(Cadastro());

        var perfil = await _sut.ObterPerfilAsync(criado.Id);

        Assert.Equal("Maria Silva", perfil.Nome);
    }

    [Fact]
    public async Task ObterPerfil_DeveFalhar_QuandoNaoExiste()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterPerfilAsync(99));
    }
}
