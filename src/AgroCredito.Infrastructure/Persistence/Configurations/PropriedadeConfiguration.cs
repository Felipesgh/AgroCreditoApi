using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroCredito.Infrastructure.Persistence.Configurations;

public class PropriedadeConfiguration : IEntityTypeConfiguration<Propriedade>
{
    public void Configure(EntityTypeBuilder<Propriedade> builder)
    {
        builder.ToTable("Propriedades");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nome).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Estado).IsRequired().HasMaxLength(2);
        builder.Property(p => p.Municipio).IsRequired().HasMaxLength(100);
        builder.Property(p => p.AreaHectares).HasPrecision(10, 2);
        builder.Property(p => p.TipoProducao).HasConversion<string>().HasMaxLength(30);
        builder.Property(p => p.Cultura).IsRequired().HasMaxLength(100);

        builder.HasOne(p => p.Produtor)
            .WithMany(p => p.Propriedades)
            .HasForeignKey(p => p.ProdutorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
