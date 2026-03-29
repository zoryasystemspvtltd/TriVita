using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrPurchaseOrderConfiguration : IEntityTypeConfiguration<PhrPurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PhrPurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrder");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PurchaseOrderNo).HasMaxLength(60);
        builder.Property(e => e.SupplierName).HasMaxLength(250);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}