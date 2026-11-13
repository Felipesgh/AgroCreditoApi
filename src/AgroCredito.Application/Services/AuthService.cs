using AgroCredito.Application.Abstractions;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Validation;
using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.Services;

public class AuthService(
    IProdutorRepository produtores,
    IPasswordHasher hasher,
    ITokenService tokens,
    IUnitOfWork unitOfWork)
{
    public async Task<ProdutorResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        AuthValidator.ValidarCadastro(request);
        var email = Normalizar(request.Email);

        if (await produtores.GetByEmailAsync(email, ct) is not null)
            throw new ConflictException("Já existe um produtor cadastrado com este e-mail.");

        var produtor = new Produtor
        {
            Nome = request.Nome.Trim(),
            Email = email,
            SenhaHash = hasher.Hash(request.Senha)
        };

        await produtores.AddAsync(produtor, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return produtor.ToResponse();
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        AuthValidator.ValidarLogin(request);

        var produtor = await produtores.GetByEmailAsync(Normalizar(request.Email), ct);
        if (produtor is null || !hasher.Verify(request.Senha, produtor.SenhaHash))
            throw new InvalidCredentialsException();

        var token = tokens.CreateToken(produtor.Id, produtor.Nome, produtor.Email);
        return new TokenResponse(token, "Bearer", tokens.ExpirationSeconds);
    }

    public async Task<ProdutorResponse> ObterPerfilAsync(int produtorId, CancellationToken ct = default)
    {
        var produtor = await produtores.GetByIdAsync(produtorId, ct)
            ?? throw new NotFoundException("Produtor não encontrado.");

        return produtor.ToResponse();
    }

    private static string Normalizar(string email) => email.Trim().ToLowerInvariant();
}
