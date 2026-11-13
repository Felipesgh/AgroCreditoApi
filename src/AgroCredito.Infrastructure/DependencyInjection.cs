using AgroCredito.Application.Abstractions;
using AgroCredito.Infrastructure.Persistence;
using AgroCredito.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AgroCredito.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AgroCreditoDbContext>(options => options.UseOracle(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPropriedadeRepository, PropriedadeRepository>();
        services.AddScoped<IOportunidadeRepository, OportunidadeRepository>();
        services.AddScoped<ISimulacaoRepository, SimulacaoRepository>();

        return services;
    }
}
