using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Enterprise;

namespace SharedService.Infrastructure.Persistence.Configurations;

public sealed class EnterpriseB2BContractConfiguration : IEntityTypeConfiguration<EnterpriseB2BContract>
{
    public void Configure(EntityTypeBuilder<EnterpriseB2BContract> builder)
    {
        builder.ToTable("EXT_EnterpriseB2BContract");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PartnerType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PartnerName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.ContractCode).HasMaxLength(80).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.EnterpriseId, e.ContractCode }).IsUnique();

        builder.HasOne<EnterpriseRoot>().WithMany().HasForeignKey(e => new { e.TenantId, e.EnterpriseId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });

        builder.HasOne<Facility>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id })
            .IsRequired(false);
    }
}
