using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsTestPackageConfiguration : IEntityTypeConfiguration<LmsTestPackage>
{
    public void Configure(EntityTypeBuilder<LmsTestPackage> builder)
    {
        builder.ToTable("LMS_TestPackage");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PackageCode).HasMaxLength(80);
        builder.Property(e => e.PackageName).HasMaxLength(250);
        builder.Property(e => e.Description).HasMaxLength(1000);
    }
}

public sealed class LmsTestPackageLineConfiguration : IEntityTypeConfiguration<LmsTestPackageLine>
{
    public void Configure(EntityTypeBuilder<LmsTestPackageLine> builder)
    {
        builder.ToTable("LMS_TestPackageLine");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class LmsTestPriceConfiguration : IEntityTypeConfiguration<LmsTestPrice>
{
    public void Configure(EntityTypeBuilder<LmsTestPrice> builder)
    {
        builder.ToTable("LMS_TestPrice");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CurrencyCode).HasMaxLength(3);
    }
}

public sealed class LmsLabInvoiceHeaderConfiguration : IEntityTypeConfiguration<LmsLabInvoiceHeader>
{
    public void Configure(EntityTypeBuilder<LmsLabInvoiceHeader> builder)
    {
        builder.ToTable("LMS_LabInvoiceHeader");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.InvoiceNo).HasMaxLength(60);
        builder.Property(e => e.CurrencyCode).HasMaxLength(3);
    }
}

public sealed class LmsLabInvoiceLineConfiguration : IEntityTypeConfiguration<LmsLabInvoiceLine>
{
    public void Configure(EntityTypeBuilder<LmsLabInvoiceLine> builder)
    {
        builder.ToTable("LMS_LabInvoiceLine");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}

public sealed class LmsLabPaymentTransactionConfiguration : IEntityTypeConfiguration<LmsLabPaymentTransaction>
{
    public void Configure(EntityTypeBuilder<LmsLabPaymentTransaction> builder)
    {
        builder.ToTable("LMS_LabPaymentTransaction");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ExternalTransactionId).HasMaxLength(200);
        builder.Property(e => e.ReferenceNo).HasMaxLength(120);
        builder.Property(e => e.Notes).HasMaxLength(500);
    }
}

public sealed class LmsPatientWalletAccountConfiguration : IEntityTypeConfiguration<LmsPatientWalletAccount>
{
    public void Configure(EntityTypeBuilder<LmsPatientWalletAccount> builder)
    {
        builder.ToTable("LMS_PatientWalletAccount");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CurrencyCode).HasMaxLength(3);
    }
}

public sealed class LmsPatientWalletTransactionConfiguration : IEntityTypeConfiguration<LmsPatientWalletTransaction>
{
    public void Configure(EntityTypeBuilder<LmsPatientWalletTransaction> builder)
    {
        builder.ToTable("LMS_PatientWalletTransaction");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ReferenceNo).HasMaxLength(120);
        builder.Property(e => e.Notes).HasMaxLength(500);
    }
}
