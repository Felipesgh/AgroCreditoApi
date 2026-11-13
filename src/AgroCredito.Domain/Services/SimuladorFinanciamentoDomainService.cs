using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Exceptions;

namespace AgroCredito.Domain.Services;

public class SimuladorFinanciamentoDomainService : ISimuladorFinanciamentoDomainService
{
    public ResultadoSimulacao Simular(Oportunidade oportunidade, decimal valor, int prazoMeses)
    {
        if (oportunidade.TaxaJurosAnual is null)
            throw new DomainException("Esta oportunidade não é uma linha de crédito simulável.");

        if (valor <= 0)
            throw new DomainException("O valor solicitado deve ser maior que zero.");

        if (prazoMeses <= 0)
            throw new DomainException("O prazo deve ser de pelo menos 1 mês.");

        if (oportunidade.ValorMaximo.HasValue && valor > oportunidade.ValorMaximo.Value)
            throw new DomainException($"O valor solicitado excede o máximo da linha ({oportunidade.ValorMaximo.Value:N2}).");

        if (oportunidade.PrazoMaximoMeses.HasValue && prazoMeses > oportunidade.PrazoMaximoMeses.Value)
            throw new DomainException($"O prazo excede o máximo da linha ({oportunidade.PrazoMaximoMeses.Value} meses).");

        var taxaAnual = (double)oportunidade.TaxaJurosAnual.Value / 100d;

        var parcela = taxaAnual == 0
            ? valor / prazoMeses
            : CalcularParcela(valor, taxaAnual, prazoMeses);

        parcela = Math.Round(parcela, 2, MidpointRounding.AwayFromZero);
        var total = parcela * prazoMeses;

        return new ResultadoSimulacao(parcela, total, total - valor);
    }

    private static decimal CalcularParcela(decimal valor, double taxaAnual, int prazoMeses)
    {
        var taxaMensal = Math.Pow(1 + taxaAnual, 1d / 12d) - 1;
        var parcela = (double)valor * taxaMensal / (1 - Math.Pow(1 + taxaMensal, -prazoMeses));
        return (decimal)parcela;
    }
}
