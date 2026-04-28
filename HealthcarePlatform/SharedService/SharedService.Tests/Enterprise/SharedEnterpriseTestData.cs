using Microsoft.EntityFrameworkCore;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Tests.Enterprise;

internal static class SharedEnterpriseTestData
{
    /// <summary>Enterprise → Company → BusinessUnit → Facility (tenant 1).</summary>
    public static (long EnterpriseId, long CompanyId, long BusinessUnitId, long FacilityId) SeedHierarchy(SharedDbContext db)
    {
        var now = DateTime.UtcNow;
        var ent = new EnterpriseRoot
        {
            TenantId = 1,
            EnterpriseCode = "E1",
            EnterpriseName = "E1",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Enterprises.Add(ent);
        db.SaveChanges();

        var comp = new Company
        {
            TenantId = 1,
            EnterpriseId = ent.Id,
            CompanyCode = "C1",
            CompanyName = "C1",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Companies.Add(comp);
        db.SaveChanges();

        var bu = new BusinessUnit
        {
            TenantId = 1,
            CompanyId = comp.Id,
            BusinessUnitCode = "BU1",
            BusinessUnitName = "BU1",
            BusinessUnitType = "Hospital",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.BusinessUnits.Add(bu);
        db.SaveChanges();

        var fac = new Facility
        {
            TenantId = 1,
            BusinessUnitId = bu.Id,
            FacilityCode = "F1",
            FacilityName = "F1",
            FacilityType = "Hospital",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Facilities.Add(fac);
        db.SaveChanges();

        return (ent.Id, comp.Id, bu.Id, fac.Id);
    }

    public static SharedDbContext CreateContext()
    {
        var cs =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DefaultConnection");

        if (string.IsNullOrWhiteSpace(cs) || !cs.Contains("Database=TriVita", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("ConnectionStrings__DefaultConnection must be set to TriVita database.");

        var options = new DbContextOptionsBuilder<SharedDbContext>()
            .UseSqlServer(cs)
            .Options;
        return new SharedDbContext(options);
    }
}
