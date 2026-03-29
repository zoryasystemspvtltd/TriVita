using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrBatchStockConfiguration : IEntityTypeConfiguration<PhrBatchStock>
{
    public void Configure(EntityTypeBuilder<PhrBatchStock> builder)
    {
        builder.ToTable("BatchStock");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CurrentQty).HasPrecision(18, 4);
        builder.Property(e => e.ReservedQty).HasPrecision(18, 4);
        builder.Property(e => e.AvailableQty).HasPrecision(18, 4);
        builder.Property(e => e.ReorderLevelQty).HasPrecision(18, 4);
    }
}