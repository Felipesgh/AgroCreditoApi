namespace AgroCredito.Domain.Entities;

public class Simulacao
{
    public int Id { get; set; }
    public int PropriedadeId { get; set; }
    public int OportunidadeId { get; set; }
    public decimal ValorSolicitado { get; set; }
    public decimal TaxaJurosAnual { get; set; }
    public int PrazoMeses { get; set; }
    public decimal ValorParcela { get; set; }
    public decimal TotalPago { get; set; }
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;

    public Propriedade? Propriedade { get; set; }
    public Oportunidade? Oportunidade { get; set; }
}