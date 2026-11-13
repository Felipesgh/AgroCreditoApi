namespace AgroCredito.Api.RateLimiting;

public sealed class RateLimitingOptions
{
    public LimiteOptions Global { get; set; } = new() { PermitLimit = 100, WindowSeconds = 60 };
    public LimiteOptions Auth { get; set; } = new() { PermitLimit = 10, WindowSeconds = 60 };
}

public sealed class LimiteOptions
{
    public int PermitLimit { get; set; }
    public int WindowSeconds { get; set; }
}
