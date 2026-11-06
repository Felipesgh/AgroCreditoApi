using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgroCredito.Infrastructure.Persistence;

namespace AgroCredito.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AgroCreditoDbContext>(options => options.UseOracle(connectionString));
        return services;
    }
}
