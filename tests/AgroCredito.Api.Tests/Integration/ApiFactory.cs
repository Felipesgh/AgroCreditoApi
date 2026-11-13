using AgroCredito.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AgroCredito.Api.Tests.Integration;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _nomeDoBanco = $"agrocredito-testes-{Guid.NewGuid()}";

    public ApiFactory() : this(null)
    {
    }

    protected ApiFactory(IDictionary<string, string>? ajustes)
    {
        var configuracao = new Dictionary<string, string>
        {
            ["ConnectionStrings__Default"] = "Data Source=nao-usado-nos-testes",
            ["Jwt__Key"] = "chave-de-testes-com-mais-de-32-caracteres-123456",
            ["RateLimiting__Global__PermitLimit"] = "100000",
            ["RateLimiting__Auth__PermitLimit"] = "100000",
            ["HealthChecks__ExternalService__Url"] = "http://localhost:1/indisponivel",
            ["HealthChecks__ExternalService__TimeoutSeconds"] = "1"
        };

        if (ajustes is not null)
        {
            foreach (var (chave, valor) in ajustes)
                configuracao[chave] = valor;
        }

        foreach (var (chave, valor) in configuracao)
            Environment.SetEnvironmentVariable(chave, valor);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var antigos = services
                .Where(d => d.ServiceType == typeof(DbContextOptions)
                         || d.ServiceType == typeof(DbContextOptions<AgroCreditoDbContext>)
                         || d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration"))
                .ToList();

            foreach (var descritor in antigos)
                services.Remove(descritor);

            services.AddDbContext<AgroCreditoDbContext>(opcoes => opcoes.UseInMemoryDatabase(_nomeDoBanco));
        });
    }
}
