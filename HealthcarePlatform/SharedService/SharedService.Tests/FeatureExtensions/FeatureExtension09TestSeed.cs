using Microsoft.EntityFrameworkCore;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;
using SharedService.Tests.Enterprise;

namespace SharedService.Tests.FeatureExtensions;

internal static class FeatureExtension09TestSeed
{
    /// <summary>Seeds Enterprise → Company → BU → Facility; returns facility id.</summary>
    public static async Task<long> SeedFacilityAsync(SharedDbContext db, long tenantId = 1L)
    {
        var now = DateTime.UtcNow;
        var ent = new EnterpriseRoot
        {
            TenantId = tenantId,
            EnterpriseCode = "FE09",
            EnterpriseName = "FE09",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Enterprises.Add(ent);
        await db.SaveChangesAsync();

        var comp = new Company
        {
            TenantId = tenantId,
            EnterpriseId = ent.Id,
            CompanyCode = "FE09C",
            CompanyName = "FE09C",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Companies.Add(comp);
        await db.SaveChangesAsync();

        var bu = new BusinessUnit
        {
            TenantId = tenantId,
            CompanyId = comp.Id,
            BusinessUnitCode = "FE09BU",
            BusinessUnitName = "FE09BU",
            BusinessUnitType = "Hospital",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.BusinessUnits.Add(bu);
        await db.SaveChangesAsync();

        var fac = new Facility
        {
            TenantId = tenantId,
            BusinessUnitId = bu.Id,
            FacilityCode = "FE09F",
            FacilityName = "FE09F",
            FacilityType = "Hospital",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Facilities.Add(fac);
        await db.SaveChangesAsync();

        return fac.Id;
    }

    public static SharedDbContext CreateInMemoryContext()
    {
        return SharedEnterpriseTestData.CreateContext();
    }
}
