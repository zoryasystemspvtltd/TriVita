using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsCollectionRequestConfiguration : IEntityTypeConfiguration<LmsCollectionRequest>
{
    public void Configure(EntityTypeBuilder<LmsCollectionRequest> builder)
    {
        builder.ToTable("LMS_CollectionRequest");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.RequestNo).HasMaxLength(60).IsRequired();
        builder.Property(e => e.CollectionAddressJson).HasColumnType("nvarchar(max)");
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.RequestNo }).IsUnique();
    }
}

public sealed class LmsRiderTrackingConfiguration : IEntityTypeConfiguration<LmsRiderTracking>
{
    public void Configure(EntityTypeBuilder<LmsRiderTracking> builder)
    {
        builder.ToTable("LMS_RiderTracking");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Latitude).HasPrecision(9, 6);
        builder.Property(e => e.Longitude).HasPrecision(9, 6);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<LmsCollectionRequest>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.CollectionRequestId })
            .HasPrincipalKey(c => new { c.TenantId, c.FacilityId, c.Id });
    }
}

public sealed class LmsSampleTransportConfiguration : IEntityTypeConfiguration<LmsSampleTransport>
{
    public void Configure(EntityTypeBuilder<LmsSampleTransport> builder)
    {
        builder.ToTable("LMS_SampleTransport");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.TemperatureCelsius).HasPrecision(5, 2);
        builder.Property(e => e.Notes).HasMaxLength(500);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<LmsCollectionRequest>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.CollectionRequestId })
            .HasPrincipalKey(c => new { c.TenantId, c.FacilityId, c.Id });
    }
}

public sealed class LmsReportValidationStepConfiguration : IEntityTypeConfiguration<LmsReportValidationStep>
{
    public void Configure(EntityTypeBuilder<LmsReportValidationStep> builder)
    {
        builder.ToTable("LMS_ReportValidationStep");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Comments).HasMaxLength(1000);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
    }
}

public sealed class LmsResultDeltaCheckConfiguration : IEntityTypeConfiguration<LmsResultDeltaCheck>
{
    public void Configure(EntityTypeBuilder<LmsResultDeltaCheck> builder)
    {
        builder.ToTable("LMS_ResultDeltaCheck");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.DeltaPercent).HasPrecision(18, 6);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
    }
}

public sealed class LmsReportDigitalSignConfiguration : IEntityTypeConfiguration<LmsReportDigitalSign>
{
    public void Configure(EntityTypeBuilder<LmsReportDigitalSign> builder)
    {
        builder.ToTable("LMS_ReportDigitalSign");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.SignatureReference).HasMaxLength(500);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
    }
}
