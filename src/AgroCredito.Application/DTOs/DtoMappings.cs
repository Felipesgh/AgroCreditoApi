using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.DTOs;

internal static class DtoMappings
{
    public static PropriedadeResponse ToResponse(this Propriedade p) =>
        new(p.Id, p.Nome, p.Estado, p.Municipio, p.AreaHectares, p.TipoProducao, p.Cultura);

    public static OportunidadeResponse ToResponse(this Oportunidade o) =>
        new(o.Id, o.Nome, o.Tipo, o.Instituicao, o.Descricao, o.FonteOficial,
            o.TaxaJurosAnual, o.ValorMaximo, o.PrazoMaximoMeses,
            o.Requisitos.Select(r => new RequisitoResponse(r.Id, r.Nome, r.Descricao)).ToList());

    public static SimulacaoResponse ToResponse(this Simulacao s, string nomeOportunidade) =>
        new(s.Id, s.PropriedadeId, s.OportunidadeId, nomeOportunidade, s.ValorSolicitado,
            s.TaxaJurosAnual, s.PrazoMeses, s.ValorParcela, s.TotalPago, s.TotalPago - s.ValorSolicitado, s.CriadaEm);
}
