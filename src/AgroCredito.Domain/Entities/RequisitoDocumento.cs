namespace AgroCredito.Domain.Entities;

public class RequisitoDocumento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int OportunidadeId { get; set; }

    public Oportunidade? Oportunidade { get; set; }
}