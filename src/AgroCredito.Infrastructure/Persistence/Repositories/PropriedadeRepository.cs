using AgroCredito.Application.Abstractions;
using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroCredito.Infrastructure.Persistence.Repositories;

public class PropriedadeRepository(AgroCreditoDbContext db) : IPropriedadeRepository
{
    public Task<Propriedade?> GetByIdAsync(int id, int produtorId, CancellationToken ct = default)
        => db.Propriedades.FirstOrDefaultAsync(p => p.Id == id && p.ProdutorId == produtorId, ct);

    public async Task<IReadOnlyList<Propriedade>> ListByProdutorAsync(int produtorId, CancellationToken ct = default)
        => await db.Propriedades
            .AsNoTracking()
            .Where(p => p.ProdutorId == produtorId)
            .OrderBy(p => p.Nome)
            .ToListAsync(ct);

    public async Task AddAsync(Propriedade propriedade, CancellationToken ct = default)
        => await db.Propriedades.AddAsync(propriedade, ct);

    public void Remove(Propriedade propriedade) => db.Propriedades.Remove(propriedade);
}
