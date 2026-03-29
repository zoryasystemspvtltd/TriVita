using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Enterprise;
using SharedService.Domain.FeatureExtensions;

namespace SharedService.Infrastructure.Persistence.Configurations;

public sealed class ModuleIntegrationHandoffConfiguration : IEntityTypeConfiguration<ModuleIntegrationHandoff>
{
    public void Configure(EntityTypeBuilder<ModuleIntegrationHandoff> builder)
    {
        builder.ToTable("EXT_ModuleIntegrationHandoff");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CorrelationId).HasMaxLength(64).IsRequired();
        builder.Property(e => e.SourceModule).HasMaxLength(30).IsRequired();
        builder.Property(e => e.TargetModule).HasMaxLength(30).IsRequired();
        builder.Property(e => e.EntityType).HasMaxLength(80).IsRequired();
        builder.Property(e => e.StatusCode).HasMaxLength(40).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.CorrelationId });

        builder.HasOne<Facility>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id })
            .IsRequired(false);
    }
}
