using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
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
        builder.Property(e => e.TransactionType).HasConversion<byte>();
        builder.Property(e => e.QuantityDelta).HasPrecision(18, 4);
        builder.Property(e => e.BeforeQty).HasPrecision(18, 4);
        builder.Property(e => e.AfterQty).HasPrecision(18, 4);
        builder.Property(e => e.UnitCost).HasPrecision(18, 4);
        builder.Property(e => e.TotalCost).HasPrecision(18, 4);
        builder.Property(e => e.SourceReference).HasMaxLength(200);
        builder.Property(e => e.Notes).HasMaxLength(1000);

        builder
            .HasOne<PhrMedicine>()
            .WithMany()
            .HasForeignKey(e => e.MedicineId)
            .OnDelete(DeleteBehavior.NoAction);
        builder
            .HasOne<PhrMedicineBatch>()
            .WithMany()
            .HasForeignKey(e => e.MedicineBatchId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.TransactionDate });
        builder.HasIndex(e => new { e.TenantId, e.ReferenceId, e.TransactionType });
        builder.HasIndex(e => new { e.TenantId, e.MedicineId, e.TransactionDate });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.MedicineBatchId, e.TransactionDate });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.GrnSupplierId });
    }
}