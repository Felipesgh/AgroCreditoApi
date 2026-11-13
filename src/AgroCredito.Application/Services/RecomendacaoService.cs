using AgroCredito.Application.Abstractions;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Domain.Services;

namespace AgroCredito.Application.Services;

public class RecomendacaoService(
    IPropriedadeRepository propriedades,
    IOportunidadeRepository oportunidades,
    IElegibilidadeDomainService elegibilidade)
{
    public const string AvisoPadrao =
        "Resultado informativo, baseado nos critérios cadastrados. Não garante aprovação de crédito: confirme as condições na fonte oficial de cada oportunidade.";

    public async Task<RecomendacaoResponse> RecomendarAsync(int propriedadeId, int produtorId, CancellationToken ct = default)
    {
        var propriedade = await propriedades.GetByIdAsync(propriedadeId, produtorId, ct)
            ?? throw new NotFoundException("Propriedade não encontrada.");

        var todas = await oportunidades.ListAsync(null, ct);
        var elegiveis = elegibilidade.FiltrarElegiveis(propriedade, todas)
            .Select(o => o.ToResponse())
            .ToList();

        return new RecomendacaoResponse(propriedade.Id, elegiveis, AvisoPadrao);
    }
}
