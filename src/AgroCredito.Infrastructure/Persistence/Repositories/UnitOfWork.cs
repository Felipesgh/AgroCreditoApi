using AgroCredito.Application.Abstractions;

namespace AgroCredito.Infrastructure.Persistence.Repositories;

public class UnitOfWork(AgroCreditoDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
