using Healthcare.Common.Responses;
using IdentityService.Application.DTOs;
using IdentityService.Application.Services;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.Rbac;
using IdentityService.Domain.Repositories;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Services;

public sealed class IdentityAdminService : IIdentityAdminService
{
    private readonly IdentityDbContext _db;
    private readonly IUserRepository _users;
    private readonly ILogger<IdentityAdminService> _logger;
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<AppUser> _hasher = new();

    public IdentityAdminService(IdentityDbContext db, IUserRepository users, ILogger<IdentityAdminService> logger)
    {
        _db = db;
        _users = users;
        _logger = logger;
    }

    public async Task<BaseResponse<long>> CreateUserAsync(
        CreateIdentityUserRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _users.EmailExistsAsync(email, request.TenantId, null, cancellationToken))
            return BaseResponse<long>.Fail("Email already exists for this tenant.");

        var user = new AppUser
        {
            Email = email,
            TenantId = request.TenantId,
            FacilityId = request.FacilityId,
            Role = request.Role,
            IsActive = request.IsActive,
        };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);
        _users.Add(user);
        await _users.SaveChangesAsync(cancellationToken);

        var profile = new IdentityUserProfile
        {
            UserId = user.Id,
            TenantId = request.TenantId,
            FacilityId = request.FacilityId,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
        };
        await _db.IdentityUserProfiles.AddAsync(profile, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<long>.Ok(user.Id);
    }

    public async Task<BaseResponse<object>> AssignUserRolesAsync(
        long userId,
        AssignUserRolesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return BaseResponse<object>.Fail("User not found.");

        await _db.IdentityUserRoles.Where(ur => ur.UserId == userId).ExecuteDeleteAsync(cancellationToken);

        foreach (var roleId in request.RoleIds.Distinct())
        {
            var role = await _db.IdentityRoles.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roleId && r.TenantId == user.TenantId && !r.IsDeleted, cancellationToken);
            if (role is null)
                return BaseResponse<object>.Fail($"Role {roleId} not found for tenant.");

            await _db.IdentityUserRoles.AddAsync(
                new IdentityUserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    TenantId = user.TenantId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                },
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User {UserId} roles replaced with {RoleIds}", userId, string.Join(",", request.RoleIds));
        return BaseResponse<object>.Ok(new { message = "Roles updated." });
    }

    public async Task<BaseResponse<object>> AssignUserFacilitiesAsync(
        long userId,
        AssignUserFacilitiesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return BaseResponse<object>.Fail("User not found.");

        await _db.IdentityUserFacilityGrants.Where(g => g.UserId == userId).ExecuteDeleteAsync(cancellationToken);

        foreach (var facId in request.GrantFacilityIds.Distinct())
        {
            await _db.IdentityUserFacilityGrants.AddAsync(
                new IdentityUserFacilityGrant
                {
                    UserId = userId,
                    TenantId = user.TenantId,
                    GrantFacilityId = facId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                },
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return BaseResponse<object>.Ok(new { message = "Facility grants updated." });
    }

    public async Task<BaseResponse<long>> CreateRoleAsync(CreateRoleRequestDto request, CancellationToken cancellationToken = default)
    {
        var code = request.RoleCode.Trim();
        if (await _db.IdentityRoles.AnyAsync(
                r => r.TenantId == request.TenantId && r.RoleCode == code && !r.IsDeleted,
                cancellationToken))
            return BaseResponse<long>.Fail("Role code already exists.");

        var role = new IdentityRole
        {
            TenantId = request.TenantId,
            RoleCode = code,
            RoleName = request.RoleName.Trim(),
            Description = request.Description,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
        };
        await _db.IdentityRoles.AddAsync(role, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return BaseResponse<long>.Ok(role.Id);
    }

    public async Task<BaseResponse<long>> CreatePermissionAsync(
        CreatePermissionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var code = request.PermissionCode.Trim();
        if (await _db.IdentityPermissions.AnyAsync(
                p => p.TenantId == request.TenantId && p.PermissionCode == code && !p.IsDeleted,
                cancellationToken))
            return BaseResponse<long>.Fail("Permission code already exists.");

        var perm = new IdentityPermission
        {
            TenantId = request.TenantId,
            PermissionCode = code,
            PermissionName = request.PermissionName.Trim(),
            ModuleCode = request.ModuleCode?.Trim(),
            Description = request.Description,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
        };
        await _db.IdentityPermissions.AddAsync(perm, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return BaseResponse<long>.Ok(perm.Id);
    }

    public async Task<BaseResponse<object>> AssignRolePermissionsAsync(
        long roleId,
        AssignRolePermissionsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var role = await _db.IdentityRoles.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted, cancellationToken);
        if (role is null)
            return BaseResponse<object>.Fail("Role not found.");

        await _db.IdentityRolePermissions.Where(rp => rp.RoleId == roleId && rp.TenantId == role.TenantId)
            .ExecuteDeleteAsync(cancellationToken);

        foreach (var permId in request.PermissionIds.Distinct())
        {
            var perm = await _db.IdentityPermissions.AsNoTracking()
                .FirstOrDefaultAsync(
                    p => p.Id == permId && p.TenantId == role.TenantId && !p.IsDeleted,
                    cancellationToken);
            if (perm is null)
                return BaseResponse<object>.Fail($"Permission {permId} not found for tenant.");

            await _db.IdentityRolePermissions.AddAsync(
                new IdentityRolePermission
                {
                    RoleId = roleId,
                    PermissionId = permId,
                    TenantId = role.TenantId,
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = DateTime.UtcNow,
                },
                cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Role {RoleId} permissions replaced with {PermissionIds}", roleId, string.Join(",", request.PermissionIds));
        return BaseResponse<object>.Ok(new { message = "Role permissions updated." });
    }
}
