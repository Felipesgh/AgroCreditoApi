namespace AgroCredito.Domain.Entities;

public class Produtor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Propriedade> Propriedades { get; set; } = new List<Propriedade>();
}
