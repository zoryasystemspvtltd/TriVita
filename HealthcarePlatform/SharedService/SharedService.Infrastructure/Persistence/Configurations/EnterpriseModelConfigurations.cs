using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.Domain.Enterprise;

namespace SharedService.Infrastructure.Persistence.Configurations;

public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Address");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Line1).HasMaxLength(250).IsRequired();
    }
}

public sealed class ContactDetailsConfiguration : IEntityTypeConfiguration<ContactDetails>
{
    public void Configure(EntityTypeBuilder<ContactDetails> builder)
    {
        builder.ToTable("ContactDetails");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ContactType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.ContactValue).HasMaxLength(300).IsRequired();
    }
}

public sealed class EnterpriseRootConfiguration : IEntityTypeConfiguration<EnterpriseRoot>
{
    public void Configure(EntityTypeBuilder<EnterpriseRoot> builder)
    {
        builder.ToTable("Enterprise");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.EnterpriseCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.EnterpriseName).HasMaxLength(250).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.EnterpriseCode }).IsUnique();

        builder.HasOne<Address>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryAddressId })
            .HasPrincipalKey(a => new { a.TenantId, a.Id });
        builder.HasOne<ContactDetails>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryContactId })
            .HasPrincipalKey(c => new { c.TenantId, c.Id });
    }
}

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CompanyCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.CompanyName).HasMaxLength(250).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.CompanyCode }).IsUnique();

        builder.HasOne<EnterpriseRoot>().WithMany().HasForeignKey(e => new { e.TenantId, e.EnterpriseId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id });
        builder.HasOne<Address>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryAddressId })
            .HasPrincipalKey(a => new { a.TenantId, a.Id });
        builder.HasOne<ContactDetails>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryContactId })
            .HasPrincipalKey(c => new { c.TenantId, c.Id });
    }
}

public sealed class BusinessUnitConfiguration : IEntityTypeConfiguration<BusinessUnit>
{
    public void Configure(EntityTypeBuilder<BusinessUnit> builder)
    {
        builder.ToTable("BusinessUnit");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BusinessUnitCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.BusinessUnitName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.BusinessUnitType).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.BusinessUnitCode }).IsUnique();

        builder.HasOne<Company>().WithMany().HasForeignKey(e => new { e.TenantId, e.CompanyId })
            .HasPrincipalKey(c => new { c.TenantId, c.Id });
        builder.HasOne<Address>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryAddressId })
            .HasPrincipalKey(a => new { a.TenantId, a.Id });
        builder.HasOne<ContactDetails>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryContactId })
            .HasPrincipalKey(c => new { c.TenantId, c.Id });
    }
}

public sealed class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("Facility");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.FacilityCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.FacilityName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.FacilityType).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.FacilityCode }).IsUnique();

        builder.HasOne<BusinessUnit>().WithMany().HasForeignKey(e => new { e.TenantId, e.BusinessUnitId })
            .HasPrincipalKey(b => new { b.TenantId, b.Id });
        builder.HasOne<Address>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryAddressId })
            .HasPrincipalKey(a => new { a.TenantId, a.Id });
        builder.HasOne<ContactDetails>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryContactId })
            .HasPrincipalKey(c => new { c.TenantId, c.Id });
    }
}

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.DepartmentCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.DepartmentName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.DepartmentType).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.DepartmentCode }).IsUnique();

        builder.HasOne<Facility>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityParentId })
            .HasPrincipalKey(f => new { f.TenantId, f.Id });

        builder.HasOne<Department>().WithMany().HasForeignKey(e => new { e.TenantId, e.ParentDepartmentId })
            .HasPrincipalKey(d => new { d.TenantId, d.Id });
        builder.HasOne<Address>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryAddressId })
            .HasPrincipalKey(a => new { a.TenantId, a.Id });
        builder.HasOne<ContactDetails>().WithMany().HasForeignKey(e => new { e.TenantId, e.PrimaryContactId })
            .HasPrincipalKey(c => new { c.TenantId, c.Id });
    }
}
