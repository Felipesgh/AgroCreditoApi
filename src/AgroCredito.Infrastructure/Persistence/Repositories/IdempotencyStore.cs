using AgroCredito.Application.Abstractions;
using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroCredito.Infrastructure.Persistence.Repositories;

public class IdempotencyStore(AgroCreditoDbContext db) : IIdempotencyStore
{
    public Task<IdempotencyRecord?> FindAsync(int produtorId, string key, CancellationToken ct = default)
        => db.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ProdutorId == produtorId && r.IdempotencyKey == key, ct);

    public async Task<bool> TryCreateAsync(IdempotencyRecord record, CancellationToken ct = default)
    {
        db.IdempotencyRecords.Add(record);
        try
        {
            await db.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException)
        {
            // Violou o índice único (produtor + chave): outra requisição reservou a chave primeiro.
            db.Entry(record).State = EntityState.Detached;
            return false;
        }
    }

    public async Task CompleteAsync(int produtorId, string key, int statusCode, string? body, string? location, CancellationToken ct = default)
    {
        var registro = await db.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.ProdutorId == produtorId && r.IdempotencyKey == key, ct);
        if (registro is null)
            return;

        registro.StatusCode = statusCode;
        registro.ResponseBody = body;
        registro.Location = location;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int produtorId, string key, CancellationToken ct = default)
    {
        var registro = await db.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.ProdutorId == produtorId && r.IdempotencyKey == key, ct);
        if (registro is null)
            return;

        db.IdempotencyRecords.Remove(registro);
        await db.SaveChangesAsync(ct);
    }
}
