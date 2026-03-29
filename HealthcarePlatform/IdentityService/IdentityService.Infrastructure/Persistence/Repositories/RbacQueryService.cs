using IdentityService.Application.Abstractions;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class RbacQueryService : IRbacQueryService
{
    private readonly IdentityDbContext _db;

    public RbacQueryService(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task<RbacPrincipalData> GetForUserAsync(long userId, long tenantId, CancellationToken cancellationToken = default)
    {
        var roleCodes = await (
                from ur in _db.IdentityUserRoles.AsNoTracking()
                join r in _db.IdentityRoles.AsNoTracking() on ur.RoleId equals r.Id
                where ur.UserId == userId
                      && ur.TenantId == tenantId
                      && r.TenantId == tenantId
                      && !ur.IsDeleted
                      && !r.IsDeleted
                      && r.IsActive
                select r.RoleCode)
            .Distinct()
            .ToListAsync(cancellationToken);

        var permissionCodes = await (
                from ur in _db.IdentityUserRoles.AsNoTracking()
                join rp in _db.IdentityRolePermissions.AsNoTracking() on ur.RoleId equals rp.RoleId
                join p in _db.IdentityPermissions.AsNoTracking() on rp.PermissionId equals p.Id
                where ur.UserId == userId
                      && ur.TenantId == tenantId
                      && rp.TenantId == tenantId
                      && p.TenantId == tenantId
                      && !ur.IsDeleted
                      && !rp.IsDeleted
                      && !p.IsDeleted
                      && p.IsActive
                select p.PermissionCode)
            .Distinct()
            .ToListAsync(cancellationToken);

        var facilityGrantIds = await _db.IdentityUserFacilityGrants.AsNoTracking()
            .Where(g => g.UserId == userId && g.TenantId == tenantId && !g.IsDeleted && g.IsActive)
            .Select(g => g.GrantFacilityId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var data = new RbacPrincipalData();
        data.RoleCodes.AddRange(roleCodes);
        data.PermissionCodes.AddRange(permissionCodes);
        data.FacilityGrantIds.AddRange(facilityGrantIds);
        return data;
    }
}
