using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroCredito.Infrastructure.Persistence.Configurations;

public class SimulacaoConfiguration : IEntityTypeConfiguration<Simulacao>
{
    public void Configure(EntityTypeBuilder<Simulacao> builder)
    {
        builder.ToTable("Simulacoes");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.ValorSolicitado).HasPrecision(18, 2);
        builder.Property(s => s.TaxaJurosAnual).HasPrecision(5, 2);
        builder.Property(s => s.ValorParcela).HasPrecision(18, 2);
        builder.Property(s => s.TotalPago).HasPrecision(18, 2);

        builder.HasOne(s => s.Propriedade)
            .WithMany(p => p.Simulacoes)
            .HasForeignKey(s => s.PropriedadeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Oportunidade)
            .WithMany()
            .HasForeignKey(s => s.OportunidadeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
