using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrPurchaseBillItemConfiguration : IEntityTypeConfiguration<PhrPurchaseBillItem>
{
    public void Configure(EntityTypeBuilder<PhrPurchaseBillItem> builder)
    {
        builder.ToTable("PurchaseBillItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Quantity).HasPrecision(18, 4);
        builder.Property(e => e.Rate).HasPrecision(18, 4);
        builder.Property(e => e.Amount).HasPrecision(18, 4);

        builder.HasIndex(e => e.GoodsReceiptItemId);

        builder.HasOne<PhrPurchaseBill>()
            .WithMany()
            .HasForeignKey(e => e.PurchaseBillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PhrGoodsReceipt>()
            .WithMany()
            .HasForeignKey(e => e.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PhrGoodsReceiptItem>()
            .WithMany()
            .HasForeignKey(e => e.GoodsReceiptItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
