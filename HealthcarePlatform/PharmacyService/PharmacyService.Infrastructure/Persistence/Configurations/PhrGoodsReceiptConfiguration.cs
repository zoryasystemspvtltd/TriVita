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
    }
}