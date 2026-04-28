using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyService.Domain.Entities;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrSupplierConfiguration : IEntityTypeConfiguration<PhrSupplier>
{
    public void Configure(EntityTypeBuilder<PhrSupplier> builder)
    {
        builder.ToTable("Supplier");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();

        builder.Property(e => e.SupplierCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.SupplierName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.Pan).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Msme).HasMaxLength(50);
        builder.Property(e => e.Tan).HasMaxLength(20);
        builder.Property(e => e.ExportImportCode).HasMaxLength(30);
        builder.Property(e => e.GstNo).HasMaxLength(15);
        builder.Property(e => e.Cin).HasMaxLength(30);
        builder.Property(e => e.ContactPerson).HasMaxLength(120);
        builder.Property(e => e.Phone).HasMaxLength(40);
        builder.Property(e => e.Email).HasMaxLength(120);
        builder.Property(e => e.Address).HasMaxLength(300);
        builder.Property(e => e.Description).HasMaxLength(500);

        builder.HasIndex(e => new { e.TenantId, e.SupplierCode }).IsUnique();
        builder.HasIndex(e => new { e.TenantId, e.SupplierName }).IsUnique();
        builder.HasIndex(e => new { e.TenantId, e.Pan }).IsUnique();
    }
}

