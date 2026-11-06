using AgroCredito.Application.Services;
using AgroCredito.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AgroCredito.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IElegibilidadeDomainService, ElegibilidadeDomainService>();
        services.AddScoped<ISimuladorFinanciamentoDomainService, SimuladorFinanciamentoDomainService>();

        services.AddScoped<AuthService>();
        services.AddScoped<PropriedadeService>();
        services.AddScoped<OportunidadeService>();
        services.AddScoped<RecomendacaoService>();
        services.AddScoped<SimulacaoService>();

        return services;
    }
}
