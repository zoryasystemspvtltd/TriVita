using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrStockLedgerConfiguration : IEntityTypeConfiguration<PhrStockLedger>
{
    public void Configure(EntityTypeBuilder<PhrStockLedger> builder)
    {
        builder.ToTable("StockLedger");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QuantityDelta).HasPrecision(18, 4);
        builder.Property(e => e.BeforeQty).HasPrecision(18, 4);
        builder.Property(e => e.AfterQty).HasPrecision(18, 4);
        builder.Property(e => e.UnitCost).HasPrecision(18, 4);
        builder.Property(e => e.TotalCost).HasPrecision(18, 4);
        builder.Property(e => e.SourceReference).HasMaxLength(200);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}