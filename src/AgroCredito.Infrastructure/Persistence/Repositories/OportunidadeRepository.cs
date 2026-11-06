using AgroCredito.Application.Abstractions;
using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgroCredito.Infrastructure.Persistence.Repositories;

public class OportunidadeRepository(AgroCreditoDbContext db) : IOportunidadeRepository
{
    // Somente leitura: AsNoTracking é mais leve. Include traz os documentos exigidos junto.
    public async Task<IReadOnlyList<Oportunidade>> ListAsync(TipoOportunidade? tipo, CancellationToken ct = default)
    {
        IQueryable<Oportunidade> query = db.Oportunidades
            .AsNoTracking()
            .Include(o => o.Requisitos);

        if (tipo.HasValue)
            query = query.Where(o => o.Tipo == tipo.Value);

        return await query.OrderBy(o => o.Nome).ToListAsync(ct);
    }

    public Task<Oportunidade?> GetByIdAsync(int id, CancellationToken ct = default)
        => db.Oportunidades
            .AsNoTracking()
            .Include(o => o.Requisitos)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
}
