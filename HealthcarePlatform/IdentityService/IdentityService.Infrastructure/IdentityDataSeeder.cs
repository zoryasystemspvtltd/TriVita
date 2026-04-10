using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.Rbac;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Healthcare.Common.Security;

namespace IdentityService.Infrastructure;

public static class IdentityDataSeeder
{
    /// <summary>Creates database (dev) and seeds demo RBAC + admin user.</summary>
    public static void Seed(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        db.Database.EnsureCreated();

        const long tenantId = 1;
        const string adminEmail = "admin@demo.local";

        if (!db.IdentityPermissions.Any(p => p.TenantId == tenantId))
        {
            var permissions = new[]
            {
                (TriVitaPermissions.HmsApi, "HMS API"),
                (TriVitaPermissions.LisApi, "LIS API"),
                (TriVitaPermissions.LmsApi, "LMS API"),
                (TriVitaPermissions.PharmacyApi, "Pharmacy API"),
                (TriVitaPermissions.SharedApi, "Shared API"),
                (TriVitaPermissions.CommunicationApi, "Communication API"),
                (TriVitaPermissions.IdentityAdmin, "Identity administration"),
                (TriVitaPermissions.Wildcard, "Full access"),
            };
            foreach (var (code, name) in permissions)
            {
                db.IdentityPermissions.Add(
                    new IdentityPermission
                    {
                        TenantId = tenantId,
                        PermissionCode = code,
                        PermissionName = name,
                        ModuleCode = "core",
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow,
                    });
            }

            db.SaveChanges();
        }

        var adminRole = db.IdentityRoles.FirstOrDefault(r => r.TenantId == tenantId && r.RoleCode == "Admin");
        if (adminRole is null)
        {
            adminRole = new IdentityRole
            {
                TenantId = tenantId,
                RoleCode = "Admin",
                RoleName = "Administrator",
                IsSystemRole = true,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
            };
            db.IdentityRoles.Add(adminRole);
            db.SaveChanges();
        }

        var permEntities = db.IdentityPermissions.Where(p => p.TenantId == tenantId && !p.IsDeleted).ToList();
        foreach (var perm in permEntities)
        {
            if (!db.IdentityRolePermissions.Any(
                    rp => rp.RoleId == adminRole.Id && rp.PermissionId == perm.Id && rp.TenantId == tenantId))
            {
                db.IdentityRolePermissions.Add(
                    new IdentityRolePermission
                    {
                        TenantId = tenantId,
                        RoleId = adminRole.Id,
                        PermissionId = perm.Id,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow,
                    });
            }
        }

        db.SaveChanges();

        AppUser? adminUser = db.Users.FirstOrDefault(u => u.Email == adminEmail && u.TenantId == tenantId);
        if (adminUser is null)
        {
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<AppUser>();
            adminUser = new AppUser
            {
                // Must match AuthService / UserRepository normalization (ToLowerInvariant).
                Email = adminEmail.ToLowerInvariant(),
                TenantId = tenantId,
                FacilityId = 1,
                Role = "Admin",
                IsActive = true,
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "ChangeMe!123");
            db.Users.Add(adminUser);
            db.SaveChanges();

            db.IdentityUserProfiles.Add(
                new IdentityUserProfile
                {
                    UserId = adminUser.Id,
                    TenantId = tenantId,
                    FacilityId = 1,
                    DisplayName = "Demo Admin",
                    MfaEnabled = false,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                });
            db.IdentityUserFacilityGrants.Add(
                new IdentityUserFacilityGrant
                {
                    UserId = adminUser.Id,
                    TenantId = tenantId,
                    GrantFacilityId = 1,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                });
            db.SaveChanges();
        }

        var ensuredUser = db.Users.FirstOrDefault(u => u.Email == adminEmail && u.TenantId == tenantId);
        if (ensuredUser is null)
            return;

        if (!db.IdentityUserRoles.Any(ur => ur.UserId == ensuredUser.Id && ur.RoleId == adminRole.Id))
        {
            db.IdentityUserRoles.Add(
                new IdentityUserRole
                {
                    UserId = ensuredUser.Id,
                    RoleId = adminRole.Id,
                    TenantId = tenantId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                });
            db.SaveChanges();
        }
    }
}
