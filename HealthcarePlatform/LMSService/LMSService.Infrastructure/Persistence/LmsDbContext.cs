using System.Reflection;
using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;
using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMSService.Infrastructure.Persistence;

public sealed class LmsDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public LmsDbContext(DbContextOptions<LmsDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<LmsProcessingStage> LmsProcessingStages => Set<LmsProcessingStage>();
    public DbSet<LmsWorkQueue> LmsWorkQueues => Set<LmsWorkQueue>();
    public DbSet<LmsTechnicianAssignment> LmsTechnicianAssignments => Set<LmsTechnicianAssignment>();
    public DbSet<LmsEquipment> LmsEquipments => Set<LmsEquipment>();
    public DbSet<LmsEquipmentMaintenance> LmsEquipmentMaintenances => Set<LmsEquipmentMaintenance>();
    public DbSet<LmsEquipmentCalibration> LmsEquipmentCalibrations => Set<LmsEquipmentCalibration>();
    public DbSet<LmsQcRecord> LmsQcRecords => Set<LmsQcRecord>();
    public DbSet<LmsQcResult> LmsQcResults => Set<LmsQcResult>();
    public DbSet<LmsLabInventory> LmsLabInventories => Set<LmsLabInventory>();
    public DbSet<LmsLabInventoryTransaction> LmsLabInventoryTransactions => Set<LmsLabInventoryTransaction>();

    public DbSet<IamUserAccount> IamUserAccounts => Set<IamUserAccount>();
    public DbSet<IamRole> IamRoles => Set<IamRole>();
    public DbSet<IamPermission> IamPermissions => Set<IamPermission>();
    public DbSet<IamRolePermission> IamRolePermissions => Set<IamRolePermission>();
    public DbSet<IamUserRoleAssignment> IamUserRoleAssignments => Set<IamUserRoleAssignment>();
    public DbSet<IamUserFacilityScope> IamUserFacilityScopes => Set<IamUserFacilityScope>();
    public DbSet<IamUserMfaFactor> IamUserMfaFactors => Set<IamUserMfaFactor>();
    public DbSet<IamPasswordResetToken> IamPasswordResetTokens => Set<IamPasswordResetToken>();
    public DbSet<IamUserSessionActivity> IamUserSessionActivities => Set<IamUserSessionActivity>();
    public DbSet<LmsTestPackage> LmsTestPackages => Set<LmsTestPackage>();
    public DbSet<LmsTestPackageLine> LmsTestPackageLines => Set<LmsTestPackageLine>();
    public DbSet<LmsTestPrice> LmsTestPrices => Set<LmsTestPrice>();
    public DbSet<LmsLabInvoiceHeader> LmsLabInvoiceHeaders => Set<LmsLabInvoiceHeader>();
    public DbSet<LmsLabInvoiceLine> LmsLabInvoiceLines => Set<LmsLabInvoiceLine>();
    public DbSet<LmsLabPaymentTransaction> LmsLabPaymentTransactions => Set<LmsLabPaymentTransaction>();
    public DbSet<LmsPatientWalletAccount> LmsPatientWalletAccounts => Set<LmsPatientWalletAccount>();
    public DbSet<LmsPatientWalletTransaction> LmsPatientWalletTransactions => Set<LmsPatientWalletTransaction>();
    public DbSet<LmsReferralDoctorProfile> LmsReferralDoctorProfiles => Set<LmsReferralDoctorProfile>();
    public DbSet<LmsReferralFeeRule> LmsReferralFeeRules => Set<LmsReferralFeeRule>();
    public DbSet<LmsReferralFeeLedger> LmsReferralFeeLedgers => Set<LmsReferralFeeLedger>();
    public DbSet<LmsReferralSettlement> LmsReferralSettlements => Set<LmsReferralSettlement>();
    public DbSet<LmsReferralSettlementLine> LmsReferralSettlementLines => Set<LmsReferralSettlementLine>();
    public DbSet<LmsB2BPartner> LmsB2BPartners => Set<LmsB2BPartner>();
    public DbSet<LmsB2BPartnerTestRate> LmsB2BPartnerTestRates => Set<LmsB2BPartnerTestRate>();
    public DbSet<LmsB2BPartnerCreditProfile> LmsB2BPartnerCreditProfiles => Set<LmsB2BPartnerCreditProfile>();
    public DbSet<LmsB2BCreditLedger> LmsB2BCreditLedgers => Set<LmsB2BCreditLedger>();
    public DbSet<LmsB2BPartnerBillingStatement> LmsB2BPartnerBillingStatements => Set<LmsB2BPartnerBillingStatement>();
    public DbSet<LmsB2BPartnerBillingStatementLine> LmsB2BPartnerBillingStatementLines => Set<LmsB2BPartnerBillingStatementLine>();
    public DbSet<LmsReagentMaster> LmsReagentMasters => Set<LmsReagentMaster>();
    public DbSet<LmsReagentBatch> LmsReagentBatches => Set<LmsReagentBatch>();
    public DbSet<LmsTestReagentMap> LmsTestReagentMaps => Set<LmsTestReagentMap>();
    public DbSet<LmsReagentConsumptionLog> LmsReagentConsumptionLogs => Set<LmsReagentConsumptionLog>();
    public DbSet<LmsLabOrderContext> LmsLabOrderContexts => Set<LmsLabOrderContext>();
    public DbSet<LmsReportPaymentGate> LmsReportPaymentGates => Set<LmsReportPaymentGate>();
    public DbSet<LmsFinanceLedgerEntry> LmsFinanceLedgerEntries => Set<LmsFinanceLedgerEntry>();
    public DbSet<LmsAnalyticsDailyFacilityRollup> LmsAnalyticsDailyFacilityRollups => Set<LmsAnalyticsDailyFacilityRollup>();
    public DbSet<SecDataChangeAuditLog> SecDataChangeAuditLogs => Set<SecDataChangeAuditLog>();

    public long TenantFilter => _tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (typeof(BaseEntity).IsAssignableFrom(clr) && clr != typeof(BaseEntity))
            {
                var configureMethod = typeof(LmsDbContext)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .First(m => m.Name == nameof(ConfigureTenantFilter) && m.IsGenericMethodDefinition);

                configureMethod.MakeGenericMethod(clr).Invoke(null, new object[] { modelBuilder, this });
            }
        }
    }

    private static void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder, LmsDbContext ctx)
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
                if (entry.Entity.FacilityId is null && _tenantContext.FacilityId is not null &&
                    !PreservesOptionalNullFacility(entry.Entity))
                    entry.Entity.FacilityId = _tenantContext.FacilityId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Schema 07 IAM / partner masters keep FacilityId null unless explicitly set.</summary>
    private static bool PreservesOptionalNullFacility(BaseEntity entity) =>
        entity is IamUserAccount or IamRole or IamPermission or IamRolePermission or IamUserRoleAssignment
            or IamUserFacilityScope or IamUserMfaFactor or IamPasswordResetToken or IamUserSessionActivity
            or SecDataChangeAuditLog or LmsB2BPartner or LmsReferralDoctorProfile;
}
