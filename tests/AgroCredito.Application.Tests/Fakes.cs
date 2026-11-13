using AgroCredito.Application.Abstractions;
using AgroCredito.Application.DTOs;
using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Enums;

namespace AgroCredito.Application.Tests;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}

internal sealed class FakePropriedadeRepository : IPropriedadeRepository
{
    private int _nextId = 1;
    public List<Propriedade> Items { get; } = new();

    public Task<Propriedade?> GetByIdAsync(int id, int produtorId, CancellationToken ct = default)
        => Task.FromResult(Items.FirstOrDefault(p => p.Id == id && p.ProdutorId == produtorId));

    public Task<IReadOnlyList<Propriedade>> ListByProdutorAsync(int produtorId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Propriedade>>(Items.Where(p => p.ProdutorId == produtorId).ToList());

    public Task AddAsync(Propriedade propriedade, CancellationToken ct = default)
    {
        propriedade.Id = _nextId++;
        Items.Add(propriedade);
        return Task.CompletedTask;
    }

    public void Remove(Propriedade propriedade) => Items.Remove(propriedade);
}

internal sealed class FakeOportunidadeRepository : IOportunidadeRepository
{
    public List<Oportunidade> Items { get; } = new();

    public Task<IReadOnlyList<Oportunidade>> ListAsync(TipoOportunidade? tipo, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Oportunidade>>(Items.Where(o => tipo == null || o.Tipo == tipo).ToList());

    public Task<Oportunidade?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(Items.FirstOrDefault(o => o.Id == id));
}

internal sealed class FakeSimulacaoRepository : ISimulacaoRepository
{
    private int _nextId = 1;
    public List<Simulacao> Items { get; } = new();

    public Task AddAsync(Simulacao simulacao, CancellationToken ct = default)
    {
        simulacao.Id = _nextId++;
        Items.Add(simulacao);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Simulacao>> ListByPropriedadeAsync(int propriedadeId, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Simulacao>>(Items.Where(s => s.PropriedadeId == propriedadeId).ToList());
}

internal static class TestData
{
    public static PropriedadeRequest PropriedadeValida() =>
        new("Sítio Boa Vista", "sp", "Ibiúna", 15m, TipoProducao.Horticultura, "Alface");

    public static Propriedade PropriedadeSp(int id, int produtorId) => new()
    {
        Id = id,
        ProdutorId = produtorId,
        Nome = "Sítio Boa Vista",
        Estado = "SP",
        Municipio = "Ibiúna",
        AreaHectares = 15m,
        TipoProducao = TipoProducao.Horticultura,
        Cultura = "Alface"
    };

    public static Oportunidade Credito(int id, decimal? taxa = 0m, string? estado = null) => new()
    {
        Id = id,
        Nome = $"Crédito {id}",
        Tipo = TipoOportunidade.CreditoRural,
        Instituicao = "Banco",
        Descricao = "Teste",
        TaxaJurosAnual = taxa,
        Estado = estado
    };
}
