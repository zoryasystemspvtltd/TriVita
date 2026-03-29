using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrPharmacySalesItemConfiguration : IEntityTypeConfiguration<PhrPharmacySalesItem>
{
    public void Configure(EntityTypeBuilder<PhrPharmacySalesItem> builder)
    {
        builder.ToTable("PharmacySalesItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QuantitySold).HasPrecision(18, 4);
        builder.Property(e => e.UnitPrice).HasPrecision(18, 4);
        builder.Property(e => e.LineTotal).HasPrecision(18, 4);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}