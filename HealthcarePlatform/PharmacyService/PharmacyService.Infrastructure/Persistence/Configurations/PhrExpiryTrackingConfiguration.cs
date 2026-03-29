using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrExpiryTrackingConfiguration : IEntityTypeConfiguration<PhrExpiryTracking>
{
    public void Configure(EntityTypeBuilder<PhrExpiryTracking> builder)
    {
        builder.ToTable("ExpiryTracking");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ExpiryDate).HasColumnType("date");
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}