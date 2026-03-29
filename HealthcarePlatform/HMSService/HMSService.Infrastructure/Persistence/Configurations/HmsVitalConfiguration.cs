using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsVitalConfiguration : IEntityTypeConfiguration<HmsVital>
{
    public void Configure(EntityTypeBuilder<HmsVital> builder)
    {
        builder.ToTable("HMS_Vitals");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ValueNumeric).HasPrecision(18, 4);
        builder.Property(e => e.ValueNumeric2).HasPrecision(18, 4);
        builder.Property(e => e.ValueText).HasMaxLength(200);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}