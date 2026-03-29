using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.FeatureExtensions;

namespace SharedService.Infrastructure.Persistence.Configurations;

public sealed class FacilityServicePriceListLineConfiguration : IEntityTypeConfiguration<FacilityServicePriceListLine>
{
    public void Configure(EntityTypeBuilder<FacilityServicePriceListLine> builder)
    {
        builder.ToTable("EXT_FacilityServicePriceListLine");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ServiceItemCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.ServiceItemName).HasMaxLength(250);
        builder.Property(e => e.TaxCategoryCode).HasMaxLength(40);
        builder.Property(e => e.UnitPrice).HasPrecision(18, 4);
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.PriceListId, e.ServiceItemCode }).IsUnique();

        builder.HasOne<FacilityServicePriceList>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.PriceListId })
            .HasPrincipalKey(p => new { p.TenantId, p.FacilityId, p.Id });
    }
}
