using System.Reflection;
using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;
using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PharmacyService.Infrastructure.Persistence;

public sealed class PharmacyDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<PhrMedicineCategory> PhrMedicineCategories => Set<PhrMedicineCategory>();
    public DbSet<PhrManufacturer> PhrManufacturers => Set<PhrManufacturer>();
    public DbSet<PhrComposition> PhrCompositions => Set<PhrComposition>();
    public DbSet<PhrForm> PhrForms => Set<PhrForm>();
    public DbSet<PhrUnit> PhrUnits => Set<PhrUnit>();
    public DbSet<PhrReferenceDataDefinition> PhrReferenceDataDefinitions => Set<PhrReferenceDataDefinition>();
    public DbSet<PhrReferenceDataValue> PhrReferenceDataValues => Set<PhrReferenceDataValue>();
    public DbSet<PhrMedicine> PhrMedicines => Set<PhrMedicine>();
    public DbSet<PhrMedicineComposition> PhrMedicineCompositions => Set<PhrMedicineComposition>();
    public DbSet<PhrMedicineBatch> PhrMedicineBatches => Set<PhrMedicineBatch>();
    public DbSet<PhrBatchStock> PhrBatchStocks => Set<PhrBatchStock>();
    public DbSet<PhrStockLedger> PhrStockLedgers => Set<PhrStockLedger>();
    public DbSet<PhrPurchaseOrder> PhrPurchaseOrders => Set<PhrPurchaseOrder>();
    public DbSet<PhrPurchaseOrderItem> PhrPurchaseOrderItems => Set<PhrPurchaseOrderItem>();
    public DbSet<PhrGoodsReceipt> PhrGoodsReceipts => Set<PhrGoodsReceipt>();
    public DbSet<PhrGoodsReceiptItem> PhrGoodsReceiptItems => Set<PhrGoodsReceiptItem>();
    public DbSet<PhrCustomer> Customers => Set<PhrCustomer>();
    public DbSet<PhrPharmacySale> PhrPharmacySales => Set<PhrPharmacySale>();
    public DbSet<PhrPharmacySalesItem> PhrPharmacySalesItems => Set<PhrPharmacySalesItem>();
    public DbSet<PhrPrescriptionMapping> PhrPrescriptionMappings => Set<PhrPrescriptionMapping>();
    public DbSet<PhrStockAdjustment> PhrStockAdjustments => Set<PhrStockAdjustment>();
    public DbSet<PhrStockAdjustmentItem> PhrStockAdjustmentItems => Set<PhrStockAdjustmentItem>();
    public DbSet<PhrStockTransfer> PhrStockTransfers => Set<PhrStockTransfer>();
    public DbSet<PhrStockTransferItem> PhrStockTransferItems => Set<PhrStockTransferItem>();
    public DbSet<PhrExpiryTracking> PhrExpiryTrackings => Set<PhrExpiryTracking>();
    public DbSet<PhrSupplier> PhrSuppliers => Set<PhrSupplier>();

    public DbSet<PhrInventoryLocation> PhrInventoryLocations => Set<PhrInventoryLocation>();
    public DbSet<PhrSalesReturn> PhrSalesReturns => Set<PhrSalesReturn>();
    public DbSet<PhrSalesReturnItem> PhrSalesReturnItems => Set<PhrSalesReturnItem>();
    public DbSet<PhrControlledDrugRegister> PhrControlledDrugRegisters => Set<PhrControlledDrugRegister>();
    public DbSet<PhrBatchStockLocation> PhrBatchStockLocations => Set<PhrBatchStockLocation>();
    public DbSet<PhrReorderPolicy> PhrReorderPolicies => Set<PhrReorderPolicy>();

    public long TenantFilter => _tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (typeof(BaseEntity).IsAssignableFrom(clr) && clr != typeof(BaseEntity))
            {
                var configureMethod = typeof(PharmacyDbContext)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .First(m => m.Name == nameof(ConfigureTenantFilter) && m.IsGenericMethodDefinition);

                configureMethod.MakeGenericMethod(clr).Invoke(null, new object[] { modelBuilder, this });
            }
        }
    }

    private static void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder, PharmacyDbContext ctx)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == ctx.TenantFilter);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.TenantId = _tenantContext.TenantId;
                if (!IsTenantCatalogOptionalFacility(entry.Entity) &&
                    entry.Entity.FacilityId is null &&
                    _tenantContext.FacilityId is not null)
                {
                    entry.Entity.FacilityId = _tenantContext.FacilityId;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static bool IsTenantCatalogOptionalFacility(BaseEntity entity) =>
        entity is PhrMedicineCategory or PhrManufacturer or PhrComposition or PhrMedicine or PhrMedicineComposition
            or PhrMedicineBatch or PhrUnit or PhrForm or PhrReferenceDataDefinition or PhrReferenceDataValue or PhrSupplier or PhrCustomer;
}
