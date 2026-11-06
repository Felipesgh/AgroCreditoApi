using AgroCredito.Domain.Entities;
using AgroCredito.Domain.Exceptions;
using AgroCredito.Domain.Services;
using Xunit;

namespace AgroCredito.Domain.Tests;

public class SimuladorFinanciamentoDomainServiceTests
{
    private readonly SimuladorFinanciamentoDomainService _service = new();

    private static Oportunidade Credito(decimal? taxa, decimal? valorMax = null, int? prazoMax = null) => new()
    {
        Nome = "Crédito teste",
        TaxaJurosAnual = taxa,
        ValorMaximo = valorMax,
        PrazoMaximoMeses = prazoMax
    };

    [Fact]
    public void Taxa_zero_divide_o_valor_igualmente()
    {
        var resultado = _service.Simular(Credito(0), 12000m, 12);

        Assert.Equal(1000m, resultado.ValorParcela);
        Assert.Equal(12000m, resultado.TotalPago);
        Assert.Equal(0m, resultado.TotalJuros);
    }

    [Fact]
    public void Calcula_parcela_pela_tabela_price()
    {
        // 6% a.a. efetivo por 12 meses: (1 + i)^-12 = 1/1,06  =>  parcela ~ 859,94
        var resultado = _service.Simular(Credito(6), 10000m, 12);

        Assert.InRange(resultado.ValorParcela, 859.90m, 859.97m);
        Assert.True(resultado.TotalJuros > 0);
    }

    [Fact]
    public void Total_pago_e_parcela_vezes_prazo()
    {
        var resultado = _service.Simular(Credito(8), 5000m, 24);

        Assert.Equal(resultado.ValorParcela * 24, resultado.TotalPago);
        Assert.Equal(resultado.TotalPago - 5000m, resultado.TotalJuros);
    }

    [Fact]
    public void Oportunidade_sem_taxa_nao_e_simulavel()
    {
        Assert.Throws<DomainException>(() => _service.Simular(Credito(null), 1000m, 12));
    }

    [Theory]
    [InlineData(0, 12)]
    [InlineData(-100, 12)]
    [InlineData(1000, 0)]
    [InlineData(1000, -3)]
    public void Valor_ou_prazo_invalidos_lancam_excecao(decimal valor, int prazo)
    {
        Assert.Throws<DomainException>(() => _service.Simular(Credito(6), valor, prazo));
    }

    [Fact]
    public void Valor_acima_do_maximo_da_linha_lanca_excecao()
    {
        Assert.Throws<DomainException>(() => _service.Simular(Credito(6, valorMax: 50000m), 60000m, 12));
    }

    [Fact]
    public void Prazo_acima_do_maximo_da_linha_lanca_excecao()
    {
        Assert.Throws<DomainException>(() => _service.Simular(Credito(6, prazoMax: 60), 10000m, 72));
    }
}