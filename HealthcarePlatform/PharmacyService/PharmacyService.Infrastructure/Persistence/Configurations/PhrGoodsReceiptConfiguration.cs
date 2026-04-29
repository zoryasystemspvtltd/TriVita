using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrGoodsReceiptConfiguration : IEntityTypeConfiguration<PhrGoodsReceipt>
{
    public void Configure(EntityTypeBuilder<PhrGoodsReceipt> builder)
    {
        builder.ToTable("GoodsReceipt");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.GoodsReceiptNo).HasMaxLength(60);
        builder.Property(e => e.Notes).HasMaxLength(1000);

        builder.Property(e => e.SubTotal).HasPrecision(18, 4);
        builder.Property(e => e.DiscountAmount).HasPrecision(18, 4);
        builder.Property(e => e.GstPercent).HasPrecision(18, 4);
        builder.Property(e => e.GstAmount).HasPrecision(18, 4);
        builder.Property(e => e.OtherTaxAmount).HasPrecision(18, 4);
        builder.Property(e => e.TotalAmount).HasPrecision(18, 4);

        builder.Property(e => e.PurchaseOrderId).IsRequired(false);
        builder.Property(e => e.SupplierId).IsRequired(false);

        // Supplier is only required in "GRN without PO" mode, but FK must exist to enforce integrity when provided.
        builder.HasOne<PhrSupplier>()
            .WithMany()
            .HasForeignKey(e => e.SupplierId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}