using AgroCredito.Domain.Enums;

namespace AgroCredito.Domain.Entities;

public class Oportunidade
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoOportunidade Tipo { get; set; }
    public string Instituicao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? FonteOficial { get; set; }
    public bool Ativa { get; set; } = true;

    public decimal? TaxaJurosAnual { get; set; }
    public decimal? ValorMaximo { get; set; }
    public int? PrazoMaximoMeses { get; set; }

    public string? Estado { get; set; }
    public TipoProducao? TipoProducao { get; set; }
    public decimal? AreaMinimaHectares { get; set; }
    public decimal? AreaMaximaHectares { get; set; }

    public ICollection<RequisitoDocumento> Requisitos { get; set; } = new List<RequisitoDocumento>();
}
