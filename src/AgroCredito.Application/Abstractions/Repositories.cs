using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Enums;

namespace AgroCredito.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IPropriedadeRepository
{
    /// <summary>Retorna null se a propriedade não existir ou for de outro produtor.</summary>
    Task<Propriedade?> GetByIdAsync(int id, int produtorId, CancellationToken ct = default);
    Task<IReadOnlyList<Propriedade>> ListByProdutorAsync(int produtorId, CancellationToken ct = default);
    Task AddAsync(Propriedade propriedade, CancellationToken ct = default);
    void Remove(Propriedade propriedade);
}

public interface IOportunidadeRepository
{
    /// <summary>Lista as oportunidades (com requisitos), opcionalmente filtradas por tipo.</summary>
    Task<IReadOnlyList<Oportunidade>> ListAsync(TipoOportunidade? tipo, CancellationToken ct = default);
    Task<Oportunidade?> GetByIdAsync(int id, CancellationToken ct = default);
}

public interface ISimulacaoRepository
{
    Task AddAsync(Simulacao simulacao, CancellationToken ct = default);
    Task<IReadOnlyList<Simulacao>> ListByPropriedadeAsync(int propriedadeId, CancellationToken ct = default);
}
