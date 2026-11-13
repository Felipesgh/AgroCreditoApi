using AgroCredito.Application.Abstractions;
using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.Tests;

internal sealed class FakeIdempotencyStore : IIdempotencyStore
{
    public List<IdempotencyRecord> Items { get; } = new();

    public Task<IdempotencyRecord?> FindAsync(int produtorId, string key, CancellationToken ct = default)
        => Task.FromResult(Items.FirstOrDefault(r => r.ProdutorId == produtorId && r.IdempotencyKey == key));

    public Task<bool> TryCreateAsync(IdempotencyRecord record, CancellationToken ct = default)
    {
        if (Items.Any(r => r.ProdutorId == record.ProdutorId && r.IdempotencyKey == record.IdempotencyKey))
            return Task.FromResult(false);

        Items.Add(record);
        return Task.FromResult(true);
    }

    public Task CompleteAsync(int produtorId, string key, int statusCode, string? body, string? location, CancellationToken ct = default)
    {
        var registro = Items.FirstOrDefault(r => r.ProdutorId == produtorId && r.IdempotencyKey == key);
        if (registro is not null)
        {
            registro.StatusCode = statusCode;
            registro.ResponseBody = body;
            registro.Location = location;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int produtorId, string key, CancellationToken ct = default)
    {
        Items.RemoveAll(r => r.ProdutorId == produtorId && r.IdempotencyKey == key);
        return Task.CompletedTask;
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset inicial) : TimeProvider
{
    public DateTimeOffset Agora { get; set; } = inicial;

    public override DateTimeOffset GetUtcNow() => Agora;
}
