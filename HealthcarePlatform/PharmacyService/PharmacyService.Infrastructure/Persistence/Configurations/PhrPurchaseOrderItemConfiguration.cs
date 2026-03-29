using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrPurchaseOrderItemConfiguration : IEntityTypeConfiguration<PhrPurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PhrPurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QuantityOrdered).HasPrecision(18, 4);
        builder.Property(e => e.PurchaseRate).HasPrecision(18, 4);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}