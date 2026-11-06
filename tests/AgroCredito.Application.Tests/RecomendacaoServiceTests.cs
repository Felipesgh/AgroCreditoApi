using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Services;
using AgroCredito.Domain.Services;
using Xunit;

namespace AgroCredito.Application.Tests;

public class RecomendacaoServiceTests
{
    private readonly FakePropriedadeRepository _props = new();
    private readonly FakeOportunidadeRepository _ops = new();
    private readonly RecomendacaoService _sut;

    public RecomendacaoServiceTests()
    {
        _props.Items.Add(TestData.PropriedadeSp(10, produtorId: 1));
        _sut = new RecomendacaoService(_props, _ops, new ElegibilidadeDomainService());
    }

    [Fact]
    public async Task Recomendar_DeveRetornarSomenteOportunidadesElegiveis()
    {
        var inativa = TestData.Credito(3);
        inativa.Ativa = false;

        _ops.Items.Add(TestData.Credito(1, estado: "SP"));
        _ops.Items.Add(TestData.Credito(2, estado: "MG"));
        _ops.Items.Add(inativa);

        var resultado = await _sut.RecomendarAsync(10, 1);

        Assert.Equal(10, resultado.PropriedadeId);
        var unica = Assert.Single(resultado.Oportunidades);
        Assert.Equal(1, unica.Id);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Aviso));
    }

    [Fact]
    public async Task Recomendar_DeveFalhar_QuandoPropriedadeEDeOutroProdutor()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.RecomendarAsync(10, 2));
    }
}
