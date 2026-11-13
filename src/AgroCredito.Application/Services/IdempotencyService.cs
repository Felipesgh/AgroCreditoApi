using AgroCredito.Application.Abstractions;
using AgroCredito.Application.Validation;
using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.Services;

public enum IdempotencyOutcome
{
    Started,
    Replay,
    InProgress,
    Mismatch
}

public sealed record IdempotencyDecision(IdempotencyOutcome Outcome, IdempotencyRecord? Record = null);

public class IdempotencyService(IIdempotencyStore store, TimeProvider clock)
{
    public static readonly TimeSpan RetencaoDasChaves = TimeSpan.FromHours(24);
    public static readonly TimeSpan LimiteEmAndamento = TimeSpan.FromSeconds(60);
    private const int TamanhoMaximoDaChave = 100;

    public async Task<IdempotencyDecision> BeginAsync(int produtorId, string key, string requestHash, CancellationToken ct = default)
    {
        ValidarChave(key);
        var agora = clock.GetUtcNow().UtcDateTime;

        var existente = await store.FindAsync(produtorId, key, ct);

        if (existente is not null && Obsoleto(existente, agora))
        {
            await store.DeleteAsync(produtorId, key, ct);
            existente = null;
        }

        if (existente is null)
        {
            var novo = new IdempotencyRecord
            {
                ProdutorId = produtorId,
                IdempotencyKey = key,
                RequestHash = requestHash,
                CriadaEm = agora
            };

            if (await store.TryCreateAsync(novo, ct))
                return new IdempotencyDecision(IdempotencyOutcome.Started);

            // Outra requisição reservou a mesma chave no mesmo instante: avalia o registro dela.
            existente = await store.FindAsync(produtorId, key, ct)
                ?? throw new InvalidOperationException("Não foi possível reservar a Idempotency-Key.");
        }

        if (existente.RequestHash != requestHash)
            return new IdempotencyDecision(IdempotencyOutcome.Mismatch);

        return existente.StatusCode is null
            ? new IdempotencyDecision(IdempotencyOutcome.InProgress)
            : new IdempotencyDecision(IdempotencyOutcome.Replay, existente);
    }

    public Task CompleteAsync(int produtorId, string key, int statusCode, string? body, string? location, CancellationToken ct = default)
        => store.CompleteAsync(produtorId, key, statusCode, body, location, ct);

    public Task ReleaseAsync(int produtorId, string key, CancellationToken ct = default)
        => store.DeleteAsync(produtorId, key, ct);

    private static bool Obsoleto(IdempotencyRecord registro, DateTime agora)
        => agora - registro.CriadaEm > (registro.StatusCode is null ? LimiteEmAndamento : RetencaoDasChaves);

    private static void ValidarChave(string key)
    {
        var erros = new ErrorCollector();

        if (string.IsNullOrWhiteSpace(key) || key.Length > TamanhoMaximoDaChave)
            erros.Add("Idempotency-Key", "A Idempotency-Key deve ter entre 1 e 100 caracteres.");

        erros.ThrowIfAny();
    }
}
