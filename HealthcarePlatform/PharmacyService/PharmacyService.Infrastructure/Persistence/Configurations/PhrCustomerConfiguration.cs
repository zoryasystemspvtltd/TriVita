using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyService.Domain.Entities;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrCustomerConfiguration : IEntityTypeConfiguration<PhrCustomer>
{
    public void Configure(EntityTypeBuilder<PhrCustomer> builder)
    {
        builder.ToTable("Customer");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();

        builder.Property(e => e.CustomerName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.MobileNumber).HasMaxLength(15).IsRequired();
        builder.Property(e => e.AlternatePhone).HasMaxLength(15);
        builder.Property(e => e.Email).HasMaxLength(120);
        builder.Property(e => e.Address).HasMaxLength(300);
        builder.Property(e => e.AadhaarNumber).HasMaxLength(12);
        builder.Property(e => e.Gender).HasMaxLength(20);

        // Unique per tenant for active (non-deleted) rows.
        builder.HasIndex(e => new { e.TenantId, e.MobileNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => new { e.TenantId, e.AadhaarNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [AadhaarNumber] IS NOT NULL");
    }
}

