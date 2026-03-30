using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsBillingHeaderConfiguration : IEntityTypeConfiguration<HmsBillingHeader>
{
    public void Configure(EntityTypeBuilder<HmsBillingHeader> builder)
    {
        builder.ToTable("HMS_BillingHeader");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.Property(e => e.FacilityId).IsRequired();
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BillNo).HasMaxLength(60);
        builder.Property(e => e.SubTotal).HasPrecision(18, 4);
        builder.Property(e => e.TaxTotal).HasPrecision(18, 4);
        builder.Property(e => e.DiscountTotal).HasPrecision(18, 4);
        builder.Property(e => e.GrandTotal).HasPrecision(18, 4);
        builder.Property(e => e.CurrencyCode).HasMaxLength(3);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}