using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroCredito.Infrastructure.Persistence.Configurations;

public class ProdutorConfiguration : IEntityTypeConfiguration<Produtor>
{
    public void Configure(EntityTypeBuilder<Produtor> builder)
    {
        builder.ToTable("Produtores");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(200);
        builder.Property(p => p.SenhaHash).IsRequired().HasMaxLength(200);
        builder.HasIndex(p => p.Email).IsUnique();
    }
}
