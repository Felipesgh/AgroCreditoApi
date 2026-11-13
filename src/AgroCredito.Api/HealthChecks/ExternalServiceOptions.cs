namespace AgroCredito.Api.HealthChecks;

public sealed class ExternalServiceOptions
{
    public string Name { get; set; } = "Banco Central do Brasil (API pública)";
    public string Url { get; set; } = "https://api.bcb.gov.br/dados/serie/bcdata.sgs.432/dados/ultimos/1?formato=json";
    public int TimeoutSeconds { get; set; } = 5;
}
