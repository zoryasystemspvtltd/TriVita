using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrInventoryLocationConfiguration : IEntityTypeConfiguration<PhrInventoryLocation>
{
    public void Configure(EntityTypeBuilder<PhrInventoryLocation> builder)
    {
        builder.ToTable("Pharmacy_InventoryLocation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.LocationCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.LocationName).HasMaxLength(250).IsRequired();
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.LocationCode }).IsUnique();
        builder.HasOne<PhrInventoryLocation>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.ParentLocationId })
            .HasPrincipalKey(p => new { p.TenantId, p.FacilityId, p.Id })
            .IsRequired(false);
    }
}

public sealed class PhrSalesReturnConfiguration : IEntityTypeConfiguration<PhrSalesReturn>
{
    public void Configure(EntityTypeBuilder<PhrSalesReturn> builder)
    {
        builder.ToTable("Pharmacy_SalesReturn");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ReturnNo).HasMaxLength(60).IsRequired();
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.ReturnNo }).IsUnique();
    }
}

public sealed class PhrSalesReturnItemConfiguration : IEntityTypeConfiguration<PhrSalesReturnItem>
{
    public void Configure(EntityTypeBuilder<PhrSalesReturnItem> builder)
    {
        builder.ToTable("Pharmacy_SalesReturnItem");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QuantityReturned).HasPrecision(18, 4);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasOne<PhrSalesReturn>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.SalesReturnId })
            .HasPrincipalKey(r => new { r.TenantId, r.FacilityId, r.Id });
    }
}

public sealed class PhrControlledDrugRegisterConfiguration : IEntityTypeConfiguration<PhrControlledDrugRegister>
{
    public void Configure(EntityTypeBuilder<PhrControlledDrugRegister> builder)
    {
        builder.ToTable("Pharmacy_ControlledDrugRegister");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
    }
}

public sealed class PhrBatchStockLocationConfiguration : IEntityTypeConfiguration<PhrBatchStockLocation>
{
    public void Configure(EntityTypeBuilder<PhrBatchStockLocation> builder)
    {
        builder.ToTable("Pharmacy_BatchStockLocation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QuantityOnHand).HasPrecision(18, 4);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.BatchStockId, e.InventoryLocationId }).IsUnique();
    }
}

public sealed class PhrReorderPolicyConfiguration : IEntityTypeConfiguration<PhrReorderPolicy>
{
    public void Configure(EntityTypeBuilder<PhrReorderPolicy> builder)
    {
        builder.ToTable("Pharmacy_ReorderPolicy");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.EconomicOrderQty).HasPrecision(18, 4);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.BatchStockId }).IsUnique();
    }
}
