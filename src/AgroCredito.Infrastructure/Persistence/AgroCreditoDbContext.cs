using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroCredito.Infrastructure.Persistence;

public class AgroCreditoDbContext(DbContextOptions<AgroCreditoDbContext> options) : DbContext(options)
{
    public DbSet<Produtor> Produtores => Set<Produtor>();
    public DbSet<Propriedade> Propriedades => Set<Propriedade>();
    public DbSet<Oportunidade> Oportunidades => Set<Oportunidade>();
    public DbSet<RequisitoDocumento> RequisitosDocumento => Set<RequisitoDocumento>();
    public DbSet<Simulacao> Simulacoes => Set<Simulacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgroCreditoDbContext).Assembly);
    }
}
