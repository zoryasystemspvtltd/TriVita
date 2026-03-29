using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrStockAdjustmentConfiguration : IEntityTypeConfiguration<PhrStockAdjustment>
{
    public void Configure(EntityTypeBuilder<PhrStockAdjustment> builder)
    {
        builder.ToTable("StockAdjustment");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.AdjustmentNo).HasMaxLength(60);
        builder.Property(e => e.ReasonNotes).HasMaxLength(1000);
    }
}