using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsPatientMasterConfiguration : IEntityTypeConfiguration<HmsPatientMaster>
{
    public void Configure(EntityTypeBuilder<HmsPatientMaster> builder)
    {
        builder.ToTable("HMS_PatientMaster");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Upid).HasColumnName("UPID").HasMaxLength(40).IsRequired();
        builder.Property(e => e.FullName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.PrimaryPhone).HasMaxLength(40);
        builder.Property(e => e.PrimaryEmail).HasMaxLength(200);
        builder.HasIndex(e => new { e.TenantId, e.Upid }).IsUnique();
    }
}

public sealed class HmsPatientFacilityLinkConfiguration : IEntityTypeConfiguration<HmsPatientFacilityLink>
{
    public void Configure(EntityTypeBuilder<HmsPatientFacilityLink> builder)
    {
        builder.ToTable("HMS_PatientFacilityLink");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasIndex(e => new { e.TenantId, e.PatientMasterId, e.FacilityId }).IsUnique();
        builder.HasOne<HmsPatientMaster>().WithMany().HasForeignKey(e => new { e.TenantId, e.PatientMasterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
    }
}

public sealed class HmsWardConfiguration : IEntityTypeConfiguration<HmsWard>
{
    public void Configure(EntityTypeBuilder<HmsWard> builder)
    {
        builder.ToTable("HMS_Ward");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.WardCode).HasMaxLength(40).IsRequired();
        builder.Property(e => e.WardName).HasMaxLength(200).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.WardCode }).IsUnique();
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
    }
}

public sealed class HmsBedConfiguration : IEntityTypeConfiguration<HmsBed>
{
    public void Configure(EntityTypeBuilder<HmsBed> builder)
    {
        builder.ToTable("HMS_Bed");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BedCode).HasMaxLength(40).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.WardId, e.BedCode }).IsUnique();
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<HmsWard>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.WardId })
            .HasPrincipalKey(w => new { w.TenantId, w.FacilityId, w.Id });
    }
}

public sealed class HmsAdmissionConfiguration : IEntityTypeConfiguration<HmsAdmission>
{
    public void Configure(EntityTypeBuilder<HmsAdmission> builder)
    {
        builder.ToTable("HMS_Admission");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.AdmissionNo).HasMaxLength(60).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.AdmissionNo }).IsUnique();
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<HmsPatientMaster>().WithMany().HasForeignKey(e => new { e.TenantId, e.PatientMasterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
        builder.HasOne<HmsBed>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.BedId })
            .HasPrincipalKey(b => new { b.TenantId, b.FacilityId, b.Id });
    }
}

public sealed class HmsAdmissionTransferConfiguration : IEntityTypeConfiguration<HmsAdmissionTransfer>
{
    public void Configure(EntityTypeBuilder<HmsAdmissionTransfer> builder)
    {
        builder.ToTable("HMS_AdmissionTransfer");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Reason).HasMaxLength(500);
        builder.HasOne<HmsAdmission>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.AdmissionId })
            .HasPrincipalKey(a => new { a.TenantId, a.FacilityId, a.Id });
    }
}

public sealed class HmsHousekeepingStatusConfiguration : IEntityTypeConfiguration<HmsHousekeepingStatus>
{
    public void Configure(EntityTypeBuilder<HmsHousekeepingStatus> builder)
    {
        builder.ToTable("HMS_HousekeepingStatus");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Notes).HasMaxLength(500);
        builder.HasOne<HmsBed>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.BedId })
            .HasPrincipalKey(b => new { b.TenantId, b.FacilityId, b.Id });
    }
}

public sealed class HmsEmarEntryConfiguration : IEntityTypeConfiguration<HmsEmarEntry>
{
    public void Configure(EntityTypeBuilder<HmsEmarEntry> builder)
    {
        builder.ToTable("HMS_EmarEntry");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.MedicationCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.Notes).HasMaxLength(500);
        builder.HasOne<HmsAdmission>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.AdmissionId })
            .HasPrincipalKey(a => new { a.TenantId, a.FacilityId, a.Id });
    }
}

public sealed class HmsDoctorOrderAlertConfiguration : IEntityTypeConfiguration<HmsDoctorOrderAlert>
{
    public void Configure(EntityTypeBuilder<HmsDoctorOrderAlert> builder)
    {
        builder.ToTable("HMS_DoctorOrderAlert");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Message).HasMaxLength(1000).IsRequired();
        builder.HasOne<HmsVisit>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.VisitId })
            .HasPrincipalKey(v => new { v.TenantId, v.FacilityId, v.Id })
            .IsRequired(false);
        builder.HasOne<HmsAdmission>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.AdmissionId })
            .HasPrincipalKey(a => new { a.TenantId, a.FacilityId, a.Id })
            .IsRequired(false);
    }
}

public sealed class HmsOperationTheatreConfiguration : IEntityTypeConfiguration<HmsOperationTheatre>
{
    public void Configure(EntityTypeBuilder<HmsOperationTheatre> builder)
    {
        builder.ToTable("HMS_OperationTheatre");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.TheatreCode).HasMaxLength(40).IsRequired();
        builder.Property(e => e.TheatreName).HasMaxLength(200).IsRequired();
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.TheatreCode }).IsUnique();
    }
}

public sealed class HmsSurgeryScheduleConfiguration : IEntityTypeConfiguration<HmsSurgerySchedule>
{
    public void Configure(EntityTypeBuilder<HmsSurgerySchedule> builder)
    {
        builder.ToTable("HMS_SurgerySchedule");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ProcedureSummary).HasMaxLength(500);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<HmsOperationTheatre>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.OperationTheatreId })
            .HasPrincipalKey(t => new { t.TenantId, t.FacilityId, t.Id });
        builder.HasOne<HmsPatientMaster>().WithMany().HasForeignKey(e => new { e.TenantId, e.PatientMasterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
    }
}

public sealed class HmsAnesthesiaRecordConfiguration : IEntityTypeConfiguration<HmsAnesthesiaRecord>
{
    public void Configure(EntityTypeBuilder<HmsAnesthesiaRecord> builder)
    {
        builder.ToTable("HMS_AnesthesiaRecord");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.RecordJson).HasColumnType("nvarchar(max)");
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<HmsSurgerySchedule>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.SurgeryScheduleId })
            .HasPrincipalKey(s => new { s.TenantId, s.FacilityId, s.Id });
    }
}

public sealed class HmsPostOpRecordConfiguration : IEntityTypeConfiguration<HmsPostOpRecord>
{
    public void Configure(EntityTypeBuilder<HmsPostOpRecord> builder)
    {
        builder.ToTable("HMS_PostOpRecord");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.RecoveryNotes).HasColumnType("nvarchar(max)");
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<HmsSurgerySchedule>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.SurgeryScheduleId })
            .HasPrincipalKey(s => new { s.TenantId, s.FacilityId, s.Id });
    }
}

public sealed class HmsOtConsumableConfiguration : IEntityTypeConfiguration<HmsOtConsumable>
{
    public void Configure(EntityTypeBuilder<HmsOtConsumable> builder)
    {
        builder.ToTable("HMS_OTConsumable");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ItemCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.ItemName).HasMaxLength(250);
        builder.Property(e => e.Quantity).HasPrecision(18, 4);
        builder.Property(e => e.UnitPrice).HasPrecision(18, 4);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<HmsSurgerySchedule>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.SurgeryScheduleId })
            .HasPrincipalKey(s => new { s.TenantId, s.FacilityId, s.Id });
    }
}

public sealed class HmsPricingRuleConfiguration : IEntityTypeConfiguration<HmsPricingRule>
{
    public void Configure(EntityTypeBuilder<HmsPricingRule> builder)
    {
        builder.ToTable("HMS_PricingRule");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.RuleCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.RuleName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.ServiceCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.UnitPrice).HasPrecision(18, 4);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.RuleCode }).IsUnique();
    }
}

public sealed class HmsPackageDefinitionConfiguration : IEntityTypeConfiguration<HmsPackageDefinition>
{
    public void Configure(EntityTypeBuilder<HmsPackageDefinition> builder)
    {
        builder.ToTable("HMS_PackageDefinition");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PackageCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.PackageName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.BundlePrice).HasPrecision(18, 4);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.PackageCode }).IsUnique();
    }
}

public sealed class HmsPackageDefinitionLineConfiguration : IEntityTypeConfiguration<HmsPackageDefinitionLine>
{
    public void Configure(EntityTypeBuilder<HmsPackageDefinitionLine> builder)
    {
        builder.ToTable("HMS_PackageDefinitionLine");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ServiceCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.Quantity).HasPrecision(18, 4);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.PackageDefinitionId, e.LineNumber }).IsUnique();
        builder.HasOne<HmsPackageDefinition>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.PackageDefinitionId })
            .HasPrincipalKey(p => new { p.TenantId, p.FacilityId, p.Id });
    }
}

public sealed class HmsProformaInvoiceConfiguration : IEntityTypeConfiguration<HmsProformaInvoice>
{
    public void Configure(EntityTypeBuilder<HmsProformaInvoice> builder)
    {
        builder.ToTable("HMS_ProformaInvoice");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ProformaNo).HasMaxLength(60).IsRequired();
        builder.Property(e => e.GrandTotal).HasPrecision(18, 4);
        builder.Property(e => e.LinesJson).HasColumnType("nvarchar(max)");
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.ProformaNo }).IsUnique();
        builder.HasOne<HmsPatientMaster>().WithMany().HasForeignKey(e => new { e.TenantId, e.PatientMasterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id })
            .IsRequired(false);
        builder.HasOne<HmsVisit>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.VisitId })
            .HasPrincipalKey(v => new { v.TenantId, v.FacilityId, v.Id })
            .IsRequired(false);
    }
}

public sealed class HmsInsuranceProviderConfiguration : IEntityTypeConfiguration<HmsInsuranceProvider>
{
    public void Configure(EntityTypeBuilder<HmsInsuranceProvider> builder)
    {
        builder.ToTable("HMS_InsuranceProvider");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ProviderCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.ProviderName).HasMaxLength(250).IsRequired();
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.ProviderCode }).IsUnique();
    }
}

public sealed class HmsPreAuthorizationConfiguration : IEntityTypeConfiguration<HmsPreAuthorization>
{
    public void Configure(EntityTypeBuilder<HmsPreAuthorization> builder)
    {
        builder.ToTable("HMS_PreAuthorization");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PreAuthNo).HasMaxLength(60).IsRequired();
        builder.Property(e => e.ApprovedAmount).HasPrecision(18, 4);
        builder.Property(e => e.Notes).HasMaxLength(2000);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.PreAuthNo }).IsUnique();
        builder.HasOne<HmsInsuranceProvider>().WithMany().HasForeignKey(e => new { e.TenantId, e.InsuranceProviderId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
        builder.HasOne<HmsPatientMaster>().WithMany().HasForeignKey(e => new { e.TenantId, e.PatientMasterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
    }
}

public sealed class HmsClaimConfiguration : IEntityTypeConfiguration<HmsClaim>
{
    public void Configure(EntityTypeBuilder<HmsClaim> builder)
    {
        builder.ToTable("HMS_Claim");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ClaimNo).HasMaxLength(60).IsRequired();
        builder.Property(e => e.ClaimAmount).HasPrecision(18, 4);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.ClaimNo }).IsUnique();
        builder.HasOne<HmsInsuranceProvider>().WithMany().HasForeignKey(e => new { e.TenantId, e.InsuranceProviderId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
        builder.HasOne<HmsPatientMaster>().WithMany().HasForeignKey(e => new { e.TenantId, e.PatientMasterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
        builder.HasOne<HmsBillingHeader>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.BillingHeaderId })
            .HasPrincipalKey(b => new { b.TenantId, b.FacilityId, b.Id })
            .IsRequired(false);
    }
}
