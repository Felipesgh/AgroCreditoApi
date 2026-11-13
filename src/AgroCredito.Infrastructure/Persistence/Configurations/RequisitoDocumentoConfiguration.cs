using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroCredito.Infrastructure.Persistence.Configurations;

public class RequisitoDocumentoConfiguration : IEntityTypeConfiguration<RequisitoDocumento>
{
    public void Configure(EntityTypeBuilder<RequisitoDocumento> builder)
    {
        builder.ToTable("RequisitosDocumento");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Nome).IsRequired().HasMaxLength(150);
        builder.Property(r => r.Descricao).IsRequired().HasMaxLength(500);

        builder.HasOne(r => r.Oportunidade)
            .WithMany(o => o.Requisitos)
            .HasForeignKey(r => r.OportunidadeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.Requisitos);
    }
}
