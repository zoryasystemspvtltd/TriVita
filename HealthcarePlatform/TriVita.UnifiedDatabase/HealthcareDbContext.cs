using System.Reflection;
using CommunicationService.Infrastructure.Persistence;
using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;
using HMSService.Domain.Entities;
using HMSService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Persistence;
using LISService.Domain.Entities;
using LISService.Infrastructure.Persistence;
using LMSService.Domain.Entities;
using LMSService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using PharmacyService.Domain.Entities;
using PharmacyService.Infrastructure.Persistence;
using SharedService.Infrastructure.Persistence;

namespace TriVita.UnifiedDatabase;

/// <summary>
/// Single-database EF Core model for TriVita HealthcarePlatform. Applies all module fluent configurations;
/// optional per-module SQL schemas are controlled by <see cref="TriVitaUnifiedModelOptions.UseModuleSchemas"/>.
/// </summary>
public sealed partial class HealthcareDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    private readonly TriVitaUnifiedModelOptions _unifiedOptions;

    public HealthcareDbContext(
        DbContextOptions<HealthcareDbContext> options,
        ITenantContext tenantContext,
        TriVitaUnifiedModelOptions? unifiedOptions = null)
        : base(options)
    {
        _tenantContext = tenantContext;
        _unifiedOptions = unifiedOptions ?? new TriVitaUnifiedModelOptions();
    }

    /// <summary>Used by global query filters (evaluated per query).</summary>
    public long TenantFilter => _tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HmsDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LisDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommunicationDbContext).Assembly);

        modelBuilder.ApplyModuleSchemas(_unifiedOptions.UseModuleSchemas);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (!ShouldApplyTenantQueryFilter(clr)) continue;

            var configureMethod = typeof(HealthcareDbContext)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .First(m => m.Name == nameof(ConfigureTenantFilter) && m.IsGenericMethodDefinition);

            configureMethod.MakeGenericMethod(clr).Invoke(null, new object[] { modelBuilder, this });
        }
    }

    /// <summary>
    /// Matches per-service contexts: tenant soft-delete filters on <see cref="BaseEntity"/> for HMS/LIS/LMS/Pharmacy/Communication only.
    /// Shared enterprise models use <see cref="SharedService.Domain.Enterprise.AuditedEntityBase"/> without this filter; Identity uses its own shape.
    /// </summary>
    private static bool ShouldApplyTenantQueryFilter(Type clr) =>
        typeof(BaseEntity).IsAssignableFrom(clr) && clr != typeof(BaseEntity)
        && IsModuleFilteredNamespace(clr.Namespace);

    private static bool IsModuleFilteredNamespace(string? ns)
    {
        if (string.IsNullOrEmpty(ns)) return false;
        return ns.StartsWith("HMSService.Domain", StringComparison.Ordinal)
               || ns.StartsWith("LISService.Domain", StringComparison.Ordinal)
               || ns.StartsWith("LMSService.Domain", StringComparison.Ordinal)
               || ns.StartsWith("PharmacyService.Domain", StringComparison.Ordinal)
               || ns.StartsWith("CommunicationService.Domain", StringComparison.Ordinal);
    }

    private static void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder, HealthcareDbContext ctx)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == ctx.TenantFilter);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State != EntityState.Added) continue;

            entry.Entity.TenantId = _tenantContext.TenantId;

            if (entry.Entity.FacilityId is not null || _tenantContext.FacilityId is null) continue;
            if (PreservesOptionalNullFacility(entry.Entity)) continue;

            entry.Entity.FacilityId = _tenantContext.FacilityId;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Union of per-service SaveChanges rules (catalog / IAM rows may keep FacilityId null).</summary>
    private static bool PreservesOptionalNullFacility(BaseEntity entity) =>
        entity is HmsPaymentMode or HmsVisitType or HmsPatientMaster or HmsInsuranceProvider
            or LisTestCategory or LisSampleType or LisTestMaster or LisTestParameter or LisTestReferenceRange
            or LisTestParameterProfile
            or IamUserAccount or IamRole or IamPermission or IamRolePermission or IamUserRoleAssignment
            or IamUserFacilityScope or IamUserMfaFactor or IamPasswordResetToken or IamUserSessionActivity
            or SecDataChangeAuditLog or LmsB2BPartner or LmsReferralDoctorProfile
            or LmsEquipmentType or LmsCatalogParameter
            or PhrMedicineCategory or PhrManufacturer or PhrComposition or PhrMedicine or PhrMedicineComposition
            or PhrMedicineBatch or PhrSupplier;
}
