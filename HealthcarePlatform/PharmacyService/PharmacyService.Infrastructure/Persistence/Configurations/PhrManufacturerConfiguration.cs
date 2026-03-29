using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrManufacturerConfiguration : IEntityTypeConfiguration<PhrManufacturer>
{
    public void Configure(EntityTypeBuilder<PhrManufacturer> builder)
    {
        builder.ToTable("Manufacturer");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ManufacturerCode).HasMaxLength(80);
        builder.Property(e => e.ManufacturerName).HasMaxLength(250);
        builder.Property(e => e.CountryCode).HasMaxLength(10);
    }
}