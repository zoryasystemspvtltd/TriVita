using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsEquipmentTypeConfiguration : IEntityTypeConfiguration<LmsEquipmentType>
{
    public void Configure(EntityTypeBuilder<LmsEquipmentType> builder)
    {
        builder.ToTable("LMS_EquipmentType");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.TypeCode).HasMaxLength(80);
        builder.Property(e => e.TypeName).HasMaxLength(250);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => new { e.TenantId, e.TypeCode }).IsUnique();
    }
}

public sealed class LmsEquipmentFacilityMappingConfiguration : IEntityTypeConfiguration<LmsEquipmentFacilityMapping>
{
    public void Configure(EntityTypeBuilder<LmsEquipmentFacilityMapping> builder)
    {
        builder.ToTable("LMS_EquipmentFacilityMapping");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.MappingNotes).HasMaxLength(500);
        builder.HasIndex(e => new { e.TenantId, e.EquipmentFacilityId, e.EquipmentId, e.MappedFacilityId }).IsUnique();
        builder.HasOne<LmsEquipment>().WithMany().HasForeignKey(e => new { e.TenantId, e.EquipmentFacilityId, e.EquipmentId })
            .HasPrincipalKey(eq => new { eq.TenantId, eq.FacilityId, eq.Id });
    }
}

public sealed class LmsCatalogTestConfiguration : IEntityTypeConfiguration<LmsCatalogTest>
{
    public void Configure(EntityTypeBuilder<LmsCatalogTest> builder)
    {
        builder.ToTable("LMS_CatalogTest");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.TestCode).HasMaxLength(80);
        builder.Property(e => e.TestName).HasMaxLength(250);
        builder.Property(e => e.TestDescription).HasMaxLength(1000);
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.TestCode }).IsUnique();
    }
}

public sealed class LmsCatalogParameterConfiguration : IEntityTypeConfiguration<LmsCatalogParameter>
{
    public void Configure(EntityTypeBuilder<LmsCatalogParameter> builder)
    {
        builder.ToTable("LMS_CatalogParameter");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ParameterCode).HasMaxLength(100);
        builder.Property(e => e.ParameterName).HasMaxLength(300);
        builder.Property(e => e.ParameterNotes).HasMaxLength(500);
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.ParameterCode }).IsUnique();
    }
}

public sealed class LmsCatalogReferenceRangeConfiguration : IEntityTypeConfiguration<LmsCatalogReferenceRange>
{
    public void Configure(EntityTypeBuilder<LmsCatalogReferenceRange> builder)
    {
        builder.ToTable("LMS_CatalogReferenceRange");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.RangeText).HasMaxLength(500);
        builder.Property(e => e.RangeNotes).HasMaxLength(800);
        builder.HasOne<LmsCatalogParameter>().WithMany().HasForeignKey(e => new { e.TenantId, e.CatalogParameterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
    }
}

public sealed class LmsCatalogTestParameterMapConfiguration : IEntityTypeConfiguration<LmsCatalogTestParameterMap>
{
    public void Configure(EntityTypeBuilder<LmsCatalogTestParameterMap> builder)
    {
        builder.ToTable("LMS_CatalogTestParameterMap");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.CatalogTestId, e.CatalogParameterId }).IsUnique();
        builder.HasOne<LmsCatalogTest>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.CatalogTestId })
            .HasPrincipalKey(t => new { t.TenantId, t.FacilityId, t.Id });
        builder.HasOne<LmsCatalogParameter>().WithMany().HasForeignKey(e => new { e.TenantId, e.CatalogParameterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
    }
}

public sealed class LmsCatalogPackageParameterMapConfiguration : IEntityTypeConfiguration<LmsCatalogPackageParameterMap>
{
    public void Configure(EntityTypeBuilder<LmsCatalogPackageParameterMap> builder)
    {
        builder.ToTable("LMS_CatalogPackageParameterMap");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasOne<LmsTestPackage>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.TestPackageId })
            .HasPrincipalKey(p => new { p.TenantId, p.FacilityId, p.Id });
        builder.HasOne<LmsCatalogParameter>().WithMany().HasForeignKey(e => new { e.TenantId, e.CatalogParameterId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
        builder.HasOne<LmsCatalogTest>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.CatalogTestId })
            .HasPrincipalKey(t => new { t.TenantId, t.FacilityId, t.Id })
            .IsRequired(false);
    }
}

public sealed class LmsEquipmentTestMasterConfiguration : IEntityTypeConfiguration<LmsEquipmentTestMaster>
{
    public void Configure(EntityTypeBuilder<LmsEquipmentTestMaster> builder)
    {
        builder.ToTable("LMS_EquipmentTestMaster");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.EquipmentAssayCode).HasMaxLength(120);
        builder.Property(e => e.EquipmentAssayName).HasMaxLength(300);
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.EquipmentId, e.EquipmentAssayCode }).IsUnique();
        builder.HasOne<LmsEquipment>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.EquipmentId })
            .HasPrincipalKey(eq => new { eq.TenantId, eq.FacilityId, eq.Id });
        builder.HasOne<LmsCatalogTest>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.CatalogTestId })
            .HasPrincipalKey(t => new { t.TenantId, t.FacilityId, t.Id });
    }
}

public sealed class LmsCatalogTestEquipmentMapConfiguration : IEntityTypeConfiguration<LmsCatalogTestEquipmentMap>
{
    public void Configure(EntityTypeBuilder<LmsCatalogTestEquipmentMap> builder)
    {
        builder.ToTable("LMS_CatalogTestEquipmentMap");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.CatalogTestId, e.EquipmentId }).IsUnique();
        builder.HasOne<LmsCatalogTest>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.CatalogTestId })
            .HasPrincipalKey(t => new { t.TenantId, t.FacilityId, t.Id });
        builder.HasOne<LmsEquipment>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.EquipmentId })
            .HasPrincipalKey(eq => new { eq.TenantId, eq.FacilityId, eq.Id });
    }
}

public sealed class LmsCatalogPackageTestLineMapConfiguration : IEntityTypeConfiguration<LmsCatalogPackageTestLineMap>
{
    public void Configure(EntityTypeBuilder<LmsCatalogPackageTestLineMap> builder)
    {
        builder.ToTable("LMS_CatalogPackageTestLineMap");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.TestPackageId, e.LineNum }).IsUnique();
        builder.HasOne<LmsTestPackage>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.TestPackageId })
            .HasPrincipalKey(p => new { p.TenantId, p.FacilityId, p.Id });
        builder.HasOne<LmsCatalogTest>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.CatalogTestId })
            .HasPrincipalKey(t => new { t.TenantId, t.FacilityId, t.Id });
    }
}

public sealed class LmsLabTestBookingConfiguration : IEntityTypeConfiguration<LmsLabTestBooking>
{
    public void Configure(EntityTypeBuilder<LmsLabTestBooking> builder)
    {
        builder.ToTable("LMS_LabTestBooking");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BookingNo).HasMaxLength(60);
        builder.Property(e => e.BookingNotes).HasMaxLength(1000);
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.BookingNo }).IsUnique();
    }
}

public sealed class LmsLabTestBookingItemConfiguration : IEntityTypeConfiguration<LmsLabTestBookingItem>
{
    public void Configure(EntityTypeBuilder<LmsLabTestBookingItem> builder)
    {
        builder.ToTable("LMS_LabTestBookingItem");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.LineNotes).HasMaxLength(500);
        builder.HasOne<LmsLabTestBooking>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.LabTestBookingId })
            .HasPrincipalKey(b => new { b.TenantId, b.FacilityId, b.Id });
        builder.HasOne<LmsCatalogTest>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.CatalogTestId })
            .HasPrincipalKey(t => new { t.TenantId, t.FacilityId, t.Id });
    }
}

public sealed class LmsLabSampleBarcodeConfiguration : IEntityTypeConfiguration<LmsLabSampleBarcode>
{
    public void Configure(EntityTypeBuilder<LmsLabSampleBarcode> builder)
    {
        builder.ToTable("LMS_LabSampleBarcode");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BarcodeValue).HasMaxLength(120);
        builder.Property(e => e.RegisteredFromSystem).HasMaxLength(120);
        builder.HasIndex(e => new { e.TenantId, e.BarcodeValue }).IsUnique();
        builder.HasOne<LmsLabTestBookingItem>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.TestBookingItemId })
            .HasPrincipalKey(i => new { i.TenantId, i.FacilityId, i.Id });
    }
}
