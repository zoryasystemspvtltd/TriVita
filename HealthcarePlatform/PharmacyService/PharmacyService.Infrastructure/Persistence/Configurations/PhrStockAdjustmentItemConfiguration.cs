using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrStockAdjustmentItemConfiguration : IEntityTypeConfiguration<PhrStockAdjustmentItem>
{
    public void Configure(EntityTypeBuilder<PhrStockAdjustmentItem> builder)
    {
        builder.ToTable("StockAdjustmentItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QuantityDelta).HasPrecision(18, 4);
        builder.Property(e => e.UnitCost).HasPrecision(18, 4);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}