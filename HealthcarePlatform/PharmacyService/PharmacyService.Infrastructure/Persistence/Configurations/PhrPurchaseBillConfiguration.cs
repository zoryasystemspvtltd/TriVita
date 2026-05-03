using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrPurchaseBillConfiguration : IEntityTypeConfiguration<PhrPurchaseBill>
{
    public void Configure(EntityTypeBuilder<PhrPurchaseBill> builder)
    {
        builder.ToTable("PurchaseBill");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BillNo).HasMaxLength(60);
        builder.Property(e => e.InvoiceNo).HasMaxLength(120);
        builder.Property(e => e.Notes).HasMaxLength(1000);

        builder.Property(e => e.SubTotal).HasPrecision(18, 4);
        builder.Property(e => e.DiscountAmount).HasPrecision(18, 4);
        builder.Property(e => e.GstPercent).HasPrecision(18, 4);
        builder.Property(e => e.GstAmount).HasPrecision(18, 4);
        builder.Property(e => e.OtherTaxAmount).HasPrecision(18, 4);
        builder.Property(e => e.NetAmount).HasPrecision(18, 4);

        builder.Property(e => e.SourceMode).HasConversion<int>();
        builder.Property(e => e.Status).HasConversion<int>();

        builder.HasIndex(e => new { e.TenantId, e.SupplierId, e.InvoiceNo })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasOne<PhrSupplier>()
            .WithMany()
            .HasForeignKey(e => e.SupplierId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<PhrPurchaseOrder>()
            .WithMany()
            .HasForeignKey(e => e.PurchaseOrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<PhrGoodsReceipt>()
            .WithMany()
            .HasForeignKey(e => e.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
