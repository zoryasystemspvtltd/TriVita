using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Enterprise;
using SharedService.Domain.FeatureExtensions;

namespace SharedService.Infrastructure.Persistence.Configurations;

public sealed class FacilityServicePriceListConfiguration : IEntityTypeConfiguration<FacilityServicePriceList>
{
    public void Configure(EntityTypeBuilder<FacilityServicePriceList> builder)
    {
        builder.ToTable("EXT_FacilityServicePriceList");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PriceListCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.PriceListName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.ServiceModule).HasMaxLength(30).IsRequired();
        builder.Property(e => e.PartnerReferenceCode).HasMaxLength(80);
        builder.Property(e => e.CurrencyCode).HasMaxLength(10).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.PriceListCode }).IsUnique();
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });

        builder.HasOne<Facility>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id });
    }
}
