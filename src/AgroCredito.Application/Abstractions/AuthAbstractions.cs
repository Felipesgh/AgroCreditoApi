using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.Abstractions;

public interface IProdutorRepository
{
    Task<Produtor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Produtor?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(Produtor produtor, CancellationToken ct = default);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface ITokenService
{
    int ExpirationSeconds { get; }
    string CreateToken(int produtorId, string nome, string email);
}
