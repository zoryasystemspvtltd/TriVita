using System.Reflection;
using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;
using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMSService.Infrastructure.Persistence;

public sealed class HmsDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public HmsDbContext(DbContextOptions<HmsDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<HmsVisitType> VisitTypes => Set<HmsVisitType>();

    public DbSet<HmsAppointment> Appointments => Set<HmsAppointment>();

    public DbSet<HmsVisit> Visits => Set<HmsVisit>();

    public DbSet<HmsAppointmentStatusHistory> AppointmentStatusHistories => Set<HmsAppointmentStatusHistory>();

    public DbSet<HmsAppointmentQueue> AppointmentQueues => Set<HmsAppointmentQueue>();

    public DbSet<HmsVital> Vitals => Set<HmsVital>();

    public DbSet<HmsClinicalNote> ClinicalNotes => Set<HmsClinicalNote>();

    public DbSet<HmsDiagnosis> Diagnoses => Set<HmsDiagnosis>();

    public DbSet<HmsMedicalProcedure> MedicalProcedures => Set<HmsMedicalProcedure>();

    public DbSet<HmsPrescription> Prescriptions => Set<HmsPrescription>();

    public DbSet<HmsPrescriptionItem> PrescriptionItems => Set<HmsPrescriptionItem>();

    public DbSet<HmsPrescriptionNote> PrescriptionNotes => Set<HmsPrescriptionNote>();

    public DbSet<HmsPaymentMode> PaymentModes => Set<HmsPaymentMode>();

    public DbSet<HmsBillingHeader> BillingHeaders => Set<HmsBillingHeader>();

    public DbSet<HmsBillingItem> BillingItems => Set<HmsBillingItem>();

    public DbSet<HmsPaymentTransaction> PaymentTransactions => Set<HmsPaymentTransaction>();

    /// <summary>Used by global query filters (evaluated per query).</summary>
    public long TenantFilter => _tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HmsDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (typeof(BaseEntity).IsAssignableFrom(clr) && clr != typeof(BaseEntity))
            {
                var configureMethod = typeof(HmsDbContext)
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .First(m => m.Name == nameof(ConfigureTenantFilter) && m.IsGenericMethodDefinition);

                configureMethod.MakeGenericMethod(clr).Invoke(null, new object[] { modelBuilder, this });
            }
        }
    }

    private static void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder, HmsDbContext ctx)
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
                // Tenant-level catalogs allow NULL FacilityId (see schema); do not force facility from context.
                if (entry.Entity is not HmsPaymentMode and not HmsVisitType &&
                    entry.Entity.FacilityId is null && _tenantContext.FacilityId is not null)
                {
                    entry.Entity.FacilityId = _tenantContext.FacilityId;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
