namespace AgroCredito.Domain.Entities;

/// <summary>Resposta guardada de uma requisição identificada por uma Idempotency-Key.</summary>
public class IdempotencyRecord
{
    public int Id { get; set; }
    public int ProdutorId { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public string RequestHash { get; set; } = string.Empty;
    public int? StatusCode { get; set; }        // null enquanto a 1ª requisição está em andamento
    public string? ResponseBody { get; set; }
    public string? Location { get; set; }
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
}
