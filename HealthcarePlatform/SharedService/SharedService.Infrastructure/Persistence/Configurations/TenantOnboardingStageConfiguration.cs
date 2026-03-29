using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Enterprise;
using SharedService.Domain.FeatureExtensions;

namespace SharedService.Infrastructure.Persistence.Configurations;

public sealed class TenantOnboardingStageConfiguration : IEntityTypeConfiguration<TenantOnboardingStage>
{
    public void Configure(EntityTypeBuilder<TenantOnboardingStage> builder)
    {
        builder.ToTable("EXT_TenantOnboardingStage");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.StageCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.StageStatus).HasMaxLength(40).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.StageCode }).IsUnique();

        builder.HasOne<Facility>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id })
            .IsRequired(false);
    }
}
