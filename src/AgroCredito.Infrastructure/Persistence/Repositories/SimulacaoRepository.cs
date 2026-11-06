using AgroCredito.Application.Abstractions;
using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroCredito.Infrastructure.Persistence.Repositories;

public class SimulacaoRepository(AgroCreditoDbContext db) : ISimulacaoRepository
{
    public async Task AddAsync(Simulacao simulacao, CancellationToken ct = default)
        => await db.Simulacoes.AddAsync(simulacao, ct);

    public async Task<IReadOnlyList<Simulacao>> ListByPropriedadeAsync(int propriedadeId, CancellationToken ct = default)
        => await db.Simulacoes
            .AsNoTracking()
            .Include(s => s.Oportunidade)
            .Where(s => s.PropriedadeId == propriedadeId)
            .OrderByDescending(s => s.CriadaEm)
            .ToListAsync(ct);
}
