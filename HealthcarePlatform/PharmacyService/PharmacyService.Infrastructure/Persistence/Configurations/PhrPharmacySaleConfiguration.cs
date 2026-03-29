using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrPharmacySaleConfiguration : IEntityTypeConfiguration<PhrPharmacySale>
{
    public void Configure(EntityTypeBuilder<PhrPharmacySale> builder)
    {
        builder.ToTable("PharmacySales");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.SalesNo).HasMaxLength(60);
        builder.Property(e => e.CurrencyCode).HasMaxLength(3);
        builder.Property(e => e.PaymentTotal).HasPrecision(18, 4);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}