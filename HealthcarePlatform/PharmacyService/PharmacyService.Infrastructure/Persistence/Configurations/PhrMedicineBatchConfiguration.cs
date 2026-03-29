using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrMedicineBatchConfiguration : IEntityTypeConfiguration<PhrMedicineBatch>
{
    public void Configure(EntityTypeBuilder<PhrMedicineBatch> builder)
    {
        builder.ToTable("MedicineBatch");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BatchNo).HasMaxLength(120);
        builder.Property(e => e.ExpiryDate).HasColumnType("date");
        builder.Property(e => e.MRP).HasPrecision(18, 4);
        builder.Property(e => e.PurchaseRate).HasPrecision(18, 4);
        builder.Property(e => e.ManufacturingDate).HasColumnType("date");
    }
}