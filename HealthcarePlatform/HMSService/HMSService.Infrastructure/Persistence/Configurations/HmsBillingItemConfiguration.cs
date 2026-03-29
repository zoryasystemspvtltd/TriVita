using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsBillingItemConfiguration : IEntityTypeConfiguration<HmsBillingItem>
{
    public void Configure(EntityTypeBuilder<HmsBillingItem> builder)
    {
        builder.ToTable("HMS_BillingItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Quantity).HasPrecision(18, 4);
        builder.Property(e => e.UnitPrice).HasPrecision(18, 4);
        builder.Property(e => e.LineTotal).HasPrecision(18, 4);
        builder.Property(e => e.ExternalReference).HasMaxLength(120);
    }
}