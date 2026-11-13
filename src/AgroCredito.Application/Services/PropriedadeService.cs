using AgroCredito.Application.Abstractions;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Validation;
using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.Services;

public class PropriedadeService(IPropriedadeRepository propriedades, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<PropriedadeResponse>> ListAsync(int produtorId, CancellationToken ct = default)
    {
        var itens = await propriedades.ListByProdutorAsync(produtorId, ct);
        return itens.Select(p => p.ToResponse()).ToList();
    }

    public async Task<PropriedadeResponse> GetAsync(int id, int produtorId, CancellationToken ct = default)
    {
        var propriedade = await ObterAsync(id, produtorId, ct);
        return propriedade.ToResponse();
    }

    public async Task<PropriedadeResponse> CreateAsync(int produtorId, PropriedadeRequest request, CancellationToken ct = default)
    {
        PropriedadeValidator.Validar(request);

        var propriedade = new Propriedade { ProdutorId = produtorId };
        Aplicar(propriedade, request);

        await propriedades.AddAsync(propriedade, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return propriedade.ToResponse();
    }

    public async Task<PropriedadeResponse> UpdateAsync(int id, int produtorId, PropriedadeRequest request, CancellationToken ct = default)
    {
        PropriedadeValidator.Validar(request);

        var propriedade = await ObterAsync(id, produtorId, ct);
        Aplicar(propriedade, request);

        await unitOfWork.SaveChangesAsync(ct);
        return propriedade.ToResponse();
    }

    public async Task DeleteAsync(int id, int produtorId, CancellationToken ct = default)
    {
        var propriedade = await ObterAsync(id, produtorId, ct);
        propriedades.Remove(propriedade);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Propriedade> ObterAsync(int id, int produtorId, CancellationToken ct)
    {
        return await propriedades.GetByIdAsync(id, produtorId, ct)
            ?? throw new NotFoundException("Propriedade não encontrada.");
    }

    private static void Aplicar(Propriedade p, PropriedadeRequest r)
    {
        p.Nome = r.Nome.Trim();
        p.Estado = r.Estado.Trim().ToUpperInvariant();
        p.Municipio = r.Municipio.Trim();
        p.AreaHectares = r.AreaHectares;
        p.TipoProducao = r.TipoProducao;
        p.Cultura = r.Cultura.Trim();
    }
}
