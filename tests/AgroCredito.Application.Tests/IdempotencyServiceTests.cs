using AgroCredito.Application.Exceptions;
using AgroCredito.Application.Services;
using Xunit;

namespace AgroCredito.Application.Tests;

public class IdempotencyServiceTests
{
    private readonly FakeIdempotencyStore _store = new();
    private readonly FixedTimeProvider _clock = new(new DateTimeOffset(2026, 11, 6, 12, 0, 0, TimeSpan.Zero));
    private readonly IdempotencyService _sut;

    public IdempotencyServiceTests()
    {
        _sut = new IdempotencyService(_store, _clock);
    }

    [Fact]
    public async Task Begin_ComChaveNova_DeveIniciarEReservarARequisicao()
    {
        var decisao = await _sut.BeginAsync(1, "chave-1", "hash-A");

        Assert.Equal(IdempotencyOutcome.Started, decisao.Outcome);
        var registro = Assert.Single(_store.Items);
        Assert.Null(registro.StatusCode);
    }

    [Fact]
    public async Task Begin_ComMesmaChaveEMesmoConteudo_AposConcluir_DeveRepetirARespostaSalva()
    {
        await _sut.BeginAsync(1, "chave-1", "hash-A");
        await _sut.CompleteAsync(1, "chave-1", 201, "{\"id\":7}", "/api/propriedades/7");

        var decisao = await _sut.BeginAsync(1, "chave-1", "hash-A");

        Assert.Equal(IdempotencyOutcome.Replay, decisao.Outcome);
        Assert.Equal(201, decisao.Record!.StatusCode);
        Assert.Equal("{\"id\":7}", decisao.Record.ResponseBody);
        Assert.Equal("/api/propriedades/7", decisao.Record.Location);
        Assert.Single(_store.Items);
    }

    [Fact]
    public async Task Begin_ComMesmaChaveEConteudoDiferente_DeveRecusar()
    {
        await _sut.BeginAsync(1, "chave-1", "hash-A");
        await _sut.CompleteAsync(1, "chave-1", 201, "{}", null);

        var decisao = await _sut.BeginAsync(1, "chave-1", "hash-B");

        Assert.Equal(IdempotencyOutcome.Mismatch, decisao.Outcome);
    }

    [Fact]
    public async Task Begin_QuandoAPrimeiraAindaEstaEmAndamento_DeveInformarConflito()
    {
        await _sut.BeginAsync(1, "chave-1", "hash-A");

        var decisao = await _sut.BeginAsync(1, "chave-1", "hash-A");

        Assert.Equal(IdempotencyOutcome.InProgress, decisao.Outcome);
    }

    [Fact]
    public async Task Begin_ComMesmaChaveDeOutroProdutor_DeveIniciarNormalmente()
    {
        await _sut.BeginAsync(1, "chave-1", "hash-A");

        var decisao = await _sut.BeginAsync(2, "chave-1", "hash-A");

        Assert.Equal(IdempotencyOutcome.Started, decisao.Outcome);
        Assert.Equal(2, _store.Items.Count);
    }

    [Fact]
    public async Task Begin_ComChaveExpirada_DeveIniciarDeNovo()
    {
        await _sut.BeginAsync(1, "chave-1", "hash-A");
        await _sut.CompleteAsync(1, "chave-1", 201, "{}", null);
        _clock.Agora = _clock.Agora.AddHours(25);

        var decisao = await _sut.BeginAsync(1, "chave-1", "hash-A");

        Assert.Equal(IdempotencyOutcome.Started, decisao.Outcome);
        Assert.Null(Assert.Single(_store.Items).StatusCode);
    }

    [Fact]
    public async Task Begin_ComExecucaoAbandonada_DeveIniciarDeNovo()
    {
        await _sut.BeginAsync(1, "chave-1", "hash-A");
        _clock.Agora = _clock.Agora.AddMinutes(2);

        var decisao = await _sut.BeginAsync(1, "chave-1", "hash-A");

        Assert.Equal(IdempotencyOutcome.Started, decisao.Outcome);
    }

    [Fact]
    public async Task Release_DeveLiberarAChaveParaNovaTentativa()
    {
        await _sut.BeginAsync(1, "chave-1", "hash-A");
        await _sut.ReleaseAsync(1, "chave-1");

        var decisao = await _sut.BeginAsync(1, "chave-1", "hash-A");

        Assert.Equal(IdempotencyOutcome.Started, decisao.Outcome);
        Assert.Single(_store.Items);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Begin_ComChaveVazia_DeveFalhar(string chave)
    {
        await Assert.ThrowsAsync<ValidationException>(() => _sut.BeginAsync(1, chave, "hash-A"));
    }

    [Fact]
    public async Task Begin_ComChaveMuitoLonga_DeveFalhar()
    {
        var ex = await Assert.ThrowsAsync<ValidationException>(() => _sut.BeginAsync(1, new string('x', 101), "hash-A"));

        Assert.Contains("Idempotency-Key", ex.Errors.Keys);
    }
}
