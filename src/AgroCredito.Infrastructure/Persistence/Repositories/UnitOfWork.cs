using AgroCredito.Application.Abstractions;

namespace AgroCredito.Infrastructure.Persistence.Repositories;

/// <summary>Confirma no banco, de uma vez, tudo que os repositórios adicionaram ou alteraram.</summary>
public class UnitOfWork(AgroCreditoDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
