using AgroCredito.Application.Abstractions;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Domain.Enums;

namespace AgroCredito.Application.Services;

public class OportunidadeService(IOportunidadeRepository oportunidades)
{
    public async Task<IReadOnlyList<OportunidadeResponse>> ListAsync(TipoOportunidade? tipo, CancellationToken ct = default)
    {
        var itens = await oportunidades.ListAsync(tipo, ct);
        return itens.Where(o => o.Ativa).Select(o => o.ToResponse()).ToList();
    }

    public async Task<OportunidadeResponse> GetAsync(int id, CancellationToken ct = default)
    {
        var oportunidade = await oportunidades.GetByIdAsync(id, ct);
        if (oportunidade is null || !oportunidade.Ativa)
            throw new NotFoundException("Oportunidade não encontrada.");

        return oportunidade.ToResponse();
    }
}
