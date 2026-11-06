using AgroCredito.Application.Abstractions;
using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroCredito.Infrastructure.Persistence.Repositories;

public class ProdutorRepository(AgroCreditoDbContext db) : IProdutorRepository
{
    public Task<Produtor?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.Produtores.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Produtor?> GetByEmailAsync(string email, CancellationToken ct = default)
        => db.Produtores.FirstOrDefaultAsync(p => p.Email == email, ct);

    public async Task AddAsync(Produtor produtor, CancellationToken ct = default)
        => await db.Produtores.AddAsync(produtor, ct);
}
