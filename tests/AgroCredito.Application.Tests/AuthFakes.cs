using AgroCredito.Application.Abstractions;
using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.Tests;

internal sealed class FakeProdutorRepository : IProdutorRepository
{
    private int _nextId = 1;
    public List<Produtor> Items { get; } = new();

    public Task<Produtor?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(Items.FirstOrDefault(p => p.Id == id));

    public Task<Produtor?> GetByEmailAsync(string email, CancellationToken ct = default)
        => Task.FromResult(Items.FirstOrDefault(p => p.Email == email));

    public Task AddAsync(Produtor produtor, CancellationToken ct = default)
    {
        produtor.Id = _nextId++;
        Items.Add(produtor);
        return Task.CompletedTask;
    }
}

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => "hash:" + password;
    public bool Verify(string password, string hash) => hash == "hash:" + password;
}

internal sealed class FakeTokenService : ITokenService
{
    public int ExpirationSeconds => 3600;
    public string CreateToken(int produtorId, string nome, string email) => $"token-{produtorId}-{email}";
}
