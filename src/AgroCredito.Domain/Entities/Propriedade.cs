using AgroCredito.Domain.Enums;

namespace AgroCredito.Domain.Entities;

public class Propriedade
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public decimal AreaHectares { get; set; }
    public TipoProducao TipoProducao { get; set; }
    public string Cultura { get; set; } = string.Empty;
    public int ProdutorId { get; set; }

    public Produtor? Produtor { get; set; }
    public ICollection<Simulacao> Simulacoes { get; set; } = new List<Simulacao>();
}