using Microsoft.EntityFrameworkCore;
using SharedService.Domain.Enterprise;
using SharedService.Domain.FeatureExtensions;

namespace SharedService.Infrastructure.Persistence;

/// <summary>EF Core context for Shared domain (enterprise hierarchy, reference data, etc.).</summary>
public sealed class SharedDbContext : DbContext
{
    public SharedDbContext(DbContextOptions<SharedDbContext> options)
        : base(options)
    {
    }

    public DbSet<Address> Addresses => Set<Address>();

    public DbSet<ContactDetails> ContactDetails => Set<ContactDetails>();

    public DbSet<EnterpriseRoot> Enterprises => Set<EnterpriseRoot>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<BusinessUnit> BusinessUnits => Set<BusinessUnit>();

    public DbSet<Facility> Facilities => Set<Facility>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<EnterpriseB2BContract> EnterpriseB2BContracts => Set<EnterpriseB2BContract>();

    public DbSet<FacilityServicePriceList> FacilityServicePriceLists => Set<FacilityServicePriceList>();

    public DbSet<FacilityServicePriceListLine> FacilityServicePriceListLines => Set<FacilityServicePriceListLine>();

    public DbSet<CrossFacilityReportAudit> CrossFacilityReportAudits => Set<CrossFacilityReportAudit>();

    public DbSet<ModuleIntegrationHandoff> ModuleIntegrationHandoffs => Set<ModuleIntegrationHandoff>();

    public DbSet<TenantOnboardingStage> TenantOnboardingStages => Set<TenantOnboardingStage>();

    public DbSet<LabCriticalValueEscalation> LabCriticalValueEscalations => Set<LabCriticalValueEscalation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);
    }
}