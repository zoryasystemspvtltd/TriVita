using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrGoodsReceiptItemConfiguration : IEntityTypeConfiguration<PhrGoodsReceiptItem>
{
    public void Configure(EntityTypeBuilder<PhrGoodsReceiptItem> builder)
    {
        builder.ToTable("GoodsReceiptItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QuantityReceived).HasPrecision(18, 4);
        builder.Property(e => e.PurchaseRate).HasPrecision(18, 4);
        builder.Property(e => e.ExpiryDate).HasColumnType("date");
        builder.Property(e => e.MRP).HasPrecision(18, 4);

        builder.Property(e => e.PurchaseOrderItemId).IsRequired(false);

        builder.HasOne<PhrPurchaseOrderItem>()
            .WithMany()
            .HasForeignKey(e => e.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}