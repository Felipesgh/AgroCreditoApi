using AgroCredito.Application.Abstractions;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Validation;
using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Exceptions;
using AgroCredito.Domain.Services;

namespace AgroCredito.Application.Services;

public class SimulacaoService(
    IPropriedadeRepository propriedades,
    IOportunidadeRepository oportunidades,
    ISimulacaoRepository simulacoes,
    ISimuladorFinanciamentoDomainService simulador,
    IElegibilidadeDomainService elegibilidade,
    IUnitOfWork unitOfWork)
{
    public async Task<SimulacaoResponse> SimularAsync(int propriedadeId, int produtorId, SimulacaoRequest request, CancellationToken ct = default)
    {
        SimulacaoValidator.Validar(request);

        var propriedade = await propriedades.GetByIdAsync(propriedadeId, produtorId, ct)
            ?? throw new NotFoundException("Propriedade não encontrada.");

        var oportunidade = await oportunidades.GetByIdAsync(request.OportunidadeId, ct)
            ?? throw new NotFoundException("Oportunidade não encontrada.");

        if (!elegibilidade.EhElegivel(propriedade, oportunidade))
            throw new DomainException("A propriedade não atende aos critérios desta oportunidade.");

        var resultado = simulador.Simular(oportunidade, request.Valor, request.PrazoMeses);

        var simulacao = new Simulacao
        {
            PropriedadeId = propriedade.Id,
            OportunidadeId = oportunidade.Id,
            ValorSolicitado = request.Valor,
            TaxaJurosAnual = oportunidade.TaxaJurosAnual ?? 0m,
            PrazoMeses = request.PrazoMeses,
            ValorParcela = resultado.ValorParcela,
            TotalPago = resultado.TotalPago
        };

        await simulacoes.AddAsync(simulacao, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return simulacao.ToResponse(oportunidade.Nome);
    }

    public async Task<IReadOnlyList<SimulacaoResponse>> ListAsync(int propriedadeId, int produtorId, CancellationToken ct = default)
    {
        var propriedade = await propriedades.GetByIdAsync(propriedadeId, produtorId, ct);
        if (propriedade is null)
            throw new NotFoundException("Propriedade não encontrada.");

        var itens = await simulacoes.ListByPropriedadeAsync(propriedadeId, ct);
        return itens.Select(s => s.ToResponse(s.Oportunidade?.Nome ?? string.Empty)).ToList();
    }
}
