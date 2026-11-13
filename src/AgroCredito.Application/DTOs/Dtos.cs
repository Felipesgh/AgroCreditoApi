using AgroCredito.Domain.Enums;

namespace AgroCredito.Application.DTOs;

public sealed record PropriedadeRequest(
    string Nome, string Estado, string Municipio, decimal AreaHectares, TipoProducao TipoProducao, string Cultura);

public sealed record PropriedadeResponse(
    int Id, string Nome, string Estado, string Municipio, decimal AreaHectares, TipoProducao TipoProducao, string Cultura);

public sealed record RequisitoResponse(int Id, string Nome, string Descricao);

public sealed record OportunidadeResponse(
    int Id, string Nome, TipoOportunidade Tipo, string Instituicao, string Descricao, string? FonteOficial,
    decimal? TaxaJurosAnual, decimal? ValorMaximo, int? PrazoMaximoMeses, IReadOnlyList<RequisitoResponse> Requisitos);

public sealed record RecomendacaoResponse(int PropriedadeId, IReadOnlyList<OportunidadeResponse> Oportunidades, string Aviso);

public sealed record SimulacaoRequest(int OportunidadeId, decimal Valor, int PrazoMeses);

public sealed record SimulacaoResponse(
    int Id, int PropriedadeId, int OportunidadeId, string Oportunidade, decimal ValorSolicitado,
    decimal TaxaJurosAnual, int PrazoMeses, decimal ValorParcela, decimal TotalPago, decimal TotalJuros, DateTime CriadaEm);
