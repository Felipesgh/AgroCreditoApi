using AgroCredito.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroCredito.Infrastructure.Persistence.Configurations;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.IdempotencyKey).IsRequired().HasMaxLength(100);
        builder.Property(r => r.RequestHash).IsRequired().HasMaxLength(64);
        builder.Property(r => r.ResponseBody).HasColumnType("NCLOB");
        builder.Property(r => r.Location).HasMaxLength(500);

        builder.HasIndex(r => new { r.ProdutorId, r.IdempotencyKey }).IsUnique();
    }
}
