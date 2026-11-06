using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroCredito.Infrastructure.Persistence.Configurations;

public class OportunidadeConfiguration : IEntityTypeConfiguration<Oportunidade>
{
    public void Configure(EntityTypeBuilder<Oportunidade> builder)
    {
        builder.ToTable("Oportunidades");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Nome).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Tipo).HasConversion<string>().HasMaxLength(30);
        builder.Property(o => o.Instituicao).IsRequired().HasMaxLength(150);
        builder.Property(o => o.Descricao).IsRequired().HasMaxLength(1000);
        builder.Property(o => o.FonteOficial).HasMaxLength(500);
        builder.Property(o => o.Ativa).HasConversion<int>();
        builder.Property(o => o.TaxaJurosAnual).HasPrecision(5, 2);
        builder.Property(o => o.ValorMaximo).HasPrecision(18, 2);
        builder.Property(o => o.Estado).HasMaxLength(2);
        builder.Property(o => o.TipoProducao).HasConversion<string>().HasMaxLength(30);
        builder.Property(o => o.AreaMinimaHectares).HasPrecision(10, 2);
        builder.Property(o => o.AreaMaximaHectares).HasPrecision(10, 2);

        builder.HasData(SeedData.Oportunidades);
    }
}

