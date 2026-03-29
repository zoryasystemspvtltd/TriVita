using System.Reflection;
using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;
using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LISService.Infrastructure.Persistence;

public sealed class LisDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public LisDbContext(DbContextOptions<LisDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<LisTestCategory> LisTestCategories => Set<LisTestCategory>();
    public DbSet<LisSampleType> LisSampleTypes => Set<LisSampleType>();
    public DbSet<LisTestMaster> LisTestMasters => Set<LisTestMaster>();
    public DbSet<LisTestParameter> LisTestParameters => Set<LisTestParameter>();
    public DbSet<LisTestReferenceRange> LisTestReferenceRanges => Set<LisTestReferenceRange>();
    public DbSet<LisLabOrder> LisLabOrders => Set<LisLabOrder>();
    public DbSet<LisLabOrderItem> LisLabOrderItems => Set<LisLabOrderItem>();
    public DbSet<LisOrderStatusHistory> LisOrderStatusHistories => Set<LisOrderStatusHistory>();
    public DbSet<LisSampleCollection> LisSampleCollections => Set<LisSampleCollection>();
    public DbSet<LisSampleTracking> LisSampleTrackings => Set<LisSampleTracking>();
    public DbSet<LisLabResult> LisLabResults => Set<LisLabResult>();
    public DbSet<LisResultApproval> LisResultApprovals => Set<LisResultApproval>();
    public DbSet<LisResultHistory> LisResultHistories => Set<LisResultHistory>();
    public DbSet<LisReportHeader> LisReportHeaders => Set<LisReportHeader>();
    public DbSet<LisReportDetail> LisReportDetails => Set<LisReportDetail>();

    public DbSet<LisTestParameterProfile> LisTestParameterProfiles => Set<LisTestParameterProfile>();
    public DbSet<LisAnalyzerResultMap> LisAnalyzerResultMaps => Set<LisAnalyzerResultMap>();
    public DbSet<LisSampleBarcode> LisSampleBarcodes => Set<LisSampleBarcode>();
    public DbSet<LisSampleLifecycleEvent> LisSampleLifecycleEvents => Set<LisSampleLifecycleEvent>();
    public DbSet<LisReportDeliveryOtp> LisReportDeliveryOtps => Set<LisReportDeliveryOtp>();
    public DbSet<LisReportLockState> LisReportLockStates => Set<LisReportLockState>();

    public long TenantFilter => _tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LisDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (typeof(BaseEntity).IsAssignableFrom(clr) && clr != typeof(BaseEntity))
            {
                var configureMethod = typeof(LisDbContext)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .First(m => m.Name == nameof(ConfigureTenantFilter) && m.IsGenericMethodDefinition);

                configureMethod.MakeGenericMethod(clr).Invoke(null, new object[] { modelBuilder, this });
            }
        }
    }

    private static void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder, LisDbContext ctx)
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
        entity is LisTestCategory or LisSampleType or LisTestMaster or LisTestParameter or LisTestReferenceRange or LisTestParameterProfile;
}
