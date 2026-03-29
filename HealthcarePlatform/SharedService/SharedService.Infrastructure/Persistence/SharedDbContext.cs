using Microsoft.EntityFrameworkCore;
using SharedService.Domain.Enterprise;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);
    }
}