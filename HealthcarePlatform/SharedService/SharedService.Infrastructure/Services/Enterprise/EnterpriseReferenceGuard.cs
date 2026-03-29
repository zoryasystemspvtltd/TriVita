using Microsoft.EntityFrameworkCore;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Services.Enterprise;

internal static class EnterpriseReferenceGuard
{
    public static Task<bool> EnterpriseExistsAsync(SharedDbContext db, long tenantId, long enterpriseId, CancellationToken ct) =>
        db.Enterprises.AsNoTracking().AnyAsync(
            e => e.Id == enterpriseId && e.TenantId == tenantId && !e.IsDeleted,
            ct);

    public static Task<bool> CompanyExistsAsync(SharedDbContext db, long tenantId, long companyId, CancellationToken ct) =>
        db.Companies.AsNoTracking().AnyAsync(
            c => c.Id == companyId && c.TenantId == tenantId && !c.IsDeleted,
            ct);

    public static Task<bool> BusinessUnitExistsAsync(SharedDbContext db, long tenantId, long businessUnitId, CancellationToken ct) =>
        db.BusinessUnits.AsNoTracking().AnyAsync(
            b => b.Id == businessUnitId && b.TenantId == tenantId && !b.IsDeleted,
            ct);

    public static Task<bool> FacilityExistsAsync(SharedDbContext db, long tenantId, long facilityId, CancellationToken ct) =>
        db.Facilities.AsNoTracking().AnyAsync(
            f => f.Id == facilityId && f.TenantId == tenantId && !f.IsDeleted,
            ct);

    public static Task<bool> AddressExistsAsync(SharedDbContext db, long tenantId, long addressId, CancellationToken ct) =>
        db.Addresses.AsNoTracking().AnyAsync(
            a => a.TenantId == tenantId && a.Id == addressId && !a.IsDeleted,
            ct);

    public static Task<bool> ContactExistsAsync(SharedDbContext db, long tenantId, long contactId, CancellationToken ct) =>
        db.ContactDetails.AsNoTracking().AnyAsync(
            c => c.TenantId == tenantId && c.Id == contactId && !c.IsDeleted,
            ct);
}
