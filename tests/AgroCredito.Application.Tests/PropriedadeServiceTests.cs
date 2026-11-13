using AgroCredito.Application.DTOs;
using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Services;
using AgroCredito.Domain.Enums;
using Xunit;

namespace AgroCredito.Application.Tests;

public class PropriedadeServiceTests
{
    private readonly FakePropriedadeRepository _repo = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly PropriedadeService _sut;

    public PropriedadeServiceTests()
    {
        _sut = new PropriedadeService(_repo, _uow);
    }

    [Fact]
    public async Task Create_DeveSalvarENormalizarEstado()
    {
        var response = await _sut.CreateAsync(1, TestData.PropriedadeValida());

        Assert.Equal("SP", response.Estado);
        Assert.Single(_repo.Items);
        Assert.Equal(1, _repo.Items[0].ProdutorId);
        Assert.Equal(1, _uow.SaveCount);
    }

    [Fact]
    public async Task Create_DeveFalhar_QuandoDadosInvalidos()
    {
        var invalida = new PropriedadeRequest("", "SPX", "Ibiúna", 0m, TipoProducao.Misto, "Alface");

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateAsync(1, invalida));

        Assert.Contains("Nome", ex.Errors.Keys);
        Assert.Contains("Estado", ex.Errors.Keys);
        Assert.Contains("AreaHectares", ex.Errors.Keys);
        Assert.Empty(_repo.Items);
        Assert.Equal(0, _uow.SaveCount);
    }

    [Fact]
    public async Task Get_DeveFalhar_QuandoPropriedadeEDeOutroProdutor()
    {
        var criada = await _sut.CreateAsync(1, TestData.PropriedadeValida());

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetAsync(criada.Id, 2));
    }

    [Fact]
    public async Task Update_DeveAlterarOsDados()
    {
        var criada = await _sut.CreateAsync(1, TestData.PropriedadeValida());
        var novos = new PropriedadeRequest("Sítio Novo", "MG", "Uberaba", 40m, TipoProducao.Agricultura, "Soja");

        var atualizada = await _sut.UpdateAsync(criada.Id, 1, novos);

        Assert.Equal("Sítio Novo", atualizada.Nome);
        Assert.Equal("MG", atualizada.Estado);
        Assert.Equal(40m, atualizada.AreaHectares);
    }

    [Fact]
    public async Task Delete_DeveRemoverAPropriedade()
    {
        var criada = await _sut.CreateAsync(1, TestData.PropriedadeValida());

        await _sut.DeleteAsync(criada.Id, 1);

        Assert.Empty(_repo.Items);
    }

    [Fact]
    public async Task List_DeveRetornarSomenteAsPropriedadesDoProdutor()
    {
        await _sut.CreateAsync(1, TestData.PropriedadeValida());
        await _sut.CreateAsync(2, TestData.PropriedadeValida());

        var lista = await _sut.ListAsync(1);

        Assert.Single(lista);
    }
}
