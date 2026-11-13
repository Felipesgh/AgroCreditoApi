using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Services;
using AgroCredito.Domain.Exceptions;
using AgroCredito.Domain.Services;
using Xunit;

namespace AgroCredito.Application.Tests;

public class SimulacaoServiceTests
{
    private readonly FakePropriedadeRepository _props = new();
    private readonly FakeOportunidadeRepository _ops = new();
    private readonly FakeSimulacaoRepository _sims = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly SimulacaoService _sut;

    public SimulacaoServiceTests()
    {
        _props.Items.Add(TestData.PropriedadeSp(10, produtorId: 1));
        _ops.Items.Add(TestData.Credito(1, taxa: 0m, estado: "SP"));
        _ops.Items.Add(TestData.Credito(2, taxa: 0m, estado: "MG"));

        _sut = new SimulacaoService(
            _props, _ops, _sims,
            new SimuladorFinanciamentoDomainService(),
            new ElegibilidadeDomainService(),
            _uow);
    }

    [Fact]
    public async Task Simular_DevePersistirEResponder()
    {
        var response = await _sut.SimularAsync(10, 1, new SimulacaoRequest(1, 12000m, 12));

        Assert.Equal(1000m, response.ValorParcela);
        Assert.Equal(12000m, response.TotalPago);
        Assert.Equal(0m, response.TotalJuros);
        Assert.Equal("Crédito 1", response.Oportunidade);
        Assert.Single(_sims.Items);
        Assert.Equal(1, _uow.SaveCount);
    }

    [Fact]
    public async Task Simular_DeveFalhar_QuandoPropriedadeNaoEElegivel()
    {
        await Assert.ThrowsAsync<DomainException>(() => _sut.SimularAsync(10, 1, new SimulacaoRequest(2, 12000m, 12)));

        Assert.Empty(_sims.Items);
    }

    [Fact]
    public async Task Simular_DeveFalhar_QuandoDadosInvalidos()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.SimularAsync(10, 1, new SimulacaoRequest(1, 0m, 12)));

        Assert.Contains("Valor", ex.Errors.Keys);
    }

    [Fact]
    public async Task Simular_DeveFalhar_QuandoOportunidadeNaoExiste()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.SimularAsync(10, 1, new SimulacaoRequest(99, 1000m, 12)));
    }

    [Fact]
    public async Task Simular_DeveFalhar_QuandoPropriedadeEDeOutroProdutor()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.SimularAsync(10, 2, new SimulacaoRequest(1, 1000m, 12)));
    }

    [Fact]
    public async Task List_DeveRetornarAsSimulacoesDaPropriedade()
    {
        await _sut.SimularAsync(10, 1, new SimulacaoRequest(1, 12000m, 12));

        var lista = await _sut.ListAsync(10, 1);

        Assert.Single(lista);
    }
}
