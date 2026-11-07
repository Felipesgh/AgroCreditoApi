using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.Abstractions;

public interface IIdempotencyStore
{
    Task<IdempotencyRecord?> FindAsync(int produtorId, string key, CancellationToken ct = default);

    /// <summary>Reserva a chave. Retorna false se outra requisição já a reservou (índice único).</summary>
    Task<bool> TryCreateAsync(IdempotencyRecord record, CancellationToken ct = default);

    Task CompleteAsync(int produtorId, string key, int statusCode, string? body, string? location, CancellationToken ct = default);
    Task DeleteAsync(int produtorId, string key, CancellationToken ct = default);
}
