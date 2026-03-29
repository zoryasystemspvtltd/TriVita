using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Enterprise;
using SharedService.Domain.FeatureExtensions;

namespace SharedService.Infrastructure.Persistence.Configurations;

public sealed class LabCriticalValueEscalationConfiguration : IEntityTypeConfiguration<LabCriticalValueEscalation>
{
    public void Configure(EntityTypeBuilder<LabCriticalValueEscalation> builder)
    {
        builder.ToTable("EXT_LabCriticalValueEscalation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ChannelCode).HasMaxLength(40).IsRequired();
        builder.Property(e => e.RecipientSummary).HasMaxLength(500);
        builder.Property(e => e.OutcomeCode).HasMaxLength(40);
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.LabResultId });

        builder.HasOne<Facility>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id });
    }
}
