using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Enterprise;
using SharedService.Domain.FeatureExtensions;

namespace SharedService.Infrastructure.Persistence.Configurations;

public sealed class CrossFacilityReportAuditConfiguration : IEntityTypeConfiguration<CrossFacilityReportAudit>
{
    public void Configure(EntityTypeBuilder<CrossFacilityReportAudit> builder)
    {
        builder.ToTable("EXT_CrossFacilityReportAudit");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ReportCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.ReportName).HasMaxLength(250);

        builder.HasOne<Facility>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id })
            .IsRequired(false);
    }
}
