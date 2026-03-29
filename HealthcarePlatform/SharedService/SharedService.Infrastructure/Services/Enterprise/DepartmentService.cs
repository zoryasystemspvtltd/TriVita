using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Services.Enterprise;

public sealed class DepartmentService : IDepartmentService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(SharedDbContext db, ITenantContext tenant, ILogger<DepartmentService> logger)
    {
        _db = db;
        _tenant = tenant;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;

    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<DepartmentResponseDto>>> ListAsync(long? facilityId, CancellationToken cancellationToken = default)
    {
        var q = _db.Departments.AsNoTracking().Where(d => d.TenantId == TenantId && !d.IsDeleted);
        if (facilityId is { } fid)
            q = q.Where(d => d.FacilityParentId == fid);

        var rows = await q.OrderBy(d => d.DepartmentCode).ToListAsync(cancellationToken);
        var items = rows.Select(d => d.ToDto()).ToList();
        return BaseResponse<IReadOnlyList<DepartmentResponseDto>>.Ok(items);
    }

    public async Task<BaseResponse<DepartmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Departments.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == TenantId && !d.IsDeleted, cancellationToken);

        return e is null
            ? BaseResponse<DepartmentResponseDto>.Fail("Department not found.")
            : BaseResponse<DepartmentResponseDto>.Ok(e.ToDto());
    }

    public async Task<BaseResponse<DepartmentResponseDto>> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        if (!await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, dto.FacilityParentId, cancellationToken))
            return BaseResponse<DepartmentResponseDto>.Fail("Facility not found for this tenant (parent constraint).");

        if (dto.ParentDepartmentId is { } pid)
        {
            var parent = await _db.Departments.AsNoTracking()
                .FirstOrDefaultAsync(
                    d => d.Id == pid && d.TenantId == TenantId && !d.IsDeleted,
                    cancellationToken);

            if (parent is null)
                return BaseResponse<DepartmentResponseDto>.Fail("Parent department not found.");

            if (parent.FacilityParentId != dto.FacilityParentId)
                return BaseResponse<DepartmentResponseDto>.Fail("Parent department must belong to the same facility.");

            if (await DepartmentParentChainIsCorruptAsync(pid, cancellationToken))
                return BaseResponse<DepartmentResponseDto>.Fail("Parent department hierarchy is cyclic (invalid hierarchy).");
        }

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<DepartmentResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<DepartmentResponseDto>.Fail("Primary contact not found for this tenant.");

        var now = DateTime.UtcNow;
        var entity = new Department
        {
            TenantId = TenantId,
            FacilityParentId = dto.FacilityParentId,
            FacilityId = dto.FacilityParentId,
            DepartmentCode = dto.DepartmentCode.Trim(),
            DepartmentName = dto.DepartmentName.Trim(),
            DepartmentType = dto.DepartmentType.Trim(),
            ParentDepartmentId = dto.ParentDepartmentId,
            PrimaryAddressId = dto.PrimaryAddressId,
            PrimaryContactId = dto.PrimaryContactId,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = AuditUser,
            ModifiedBy = AuditUser
        };

        _db.Departments.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Department created TenantId={TenantId} DepartmentId={Id} Code={Code}",
            TenantId,
            entity.Id,
            entity.DepartmentCode);

        return BaseResponse<DepartmentResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<DepartmentResponseDto>> UpdateAsync(long id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Departments.FirstOrDefaultAsync(
            d => d.Id == id && d.TenantId == TenantId && !d.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<DepartmentResponseDto>.Fail("Department not found.");

        if (!await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, dto.FacilityParentId, cancellationToken))
            return BaseResponse<DepartmentResponseDto>.Fail("Facility not found for this tenant (parent constraint).");

        if (dto.ParentDepartmentId is { } pid)
        {
            if (pid == id)
            {
                _logger.LogWarning(
                    "Invalid department self-parent TenantId={TenantId} DepartmentId={DepartmentId}",
                    TenantId,
                    id);
                return BaseResponse<DepartmentResponseDto>.Fail("Department cannot be its own parent (circular hierarchy).");
            }

            var parent = await _db.Departments.AsNoTracking()
                .FirstOrDefaultAsync(
                    d => d.Id == pid && d.TenantId == TenantId && !d.IsDeleted,
                    cancellationToken);

            if (parent is null)
                return BaseResponse<DepartmentResponseDto>.Fail("Parent department not found.");

            if (parent.FacilityParentId != dto.FacilityParentId)
                return BaseResponse<DepartmentResponseDto>.Fail("Parent department must belong to the same facility.");

            if (await DepartmentHierarchyWouldCycleAsync(id, pid, cancellationToken))
            {
                _logger.LogWarning(
                    "Department circular hierarchy attempt TenantId={TenantId} DepartmentId={DepartmentId} NewParent={ParentId}",
                    TenantId,
                    id,
                    pid);
                return BaseResponse<DepartmentResponseDto>.Fail("Department parent assignment would create a circular hierarchy.");
            }
        }

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<DepartmentResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<DepartmentResponseDto>.Fail("Primary contact not found for this tenant.");

        entity.FacilityParentId = dto.FacilityParentId;
        entity.FacilityId = dto.FacilityParentId;
        entity.DepartmentCode = dto.DepartmentCode.Trim();
        entity.DepartmentName = dto.DepartmentName.Trim();
        entity.DepartmentType = dto.DepartmentType.Trim();
        entity.ParentDepartmentId = dto.ParentDepartmentId;
        entity.PrimaryAddressId = dto.PrimaryAddressId;
        entity.PrimaryContactId = dto.PrimaryContactId;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveTo = dto.EffectiveTo;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Department updated TenantId={TenantId} DepartmentId={Id}", TenantId, id);

        return BaseResponse<DepartmentResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Departments.FirstOrDefaultAsync(
            d => d.Id == id && d.TenantId == TenantId && !d.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Department not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Department soft-deleted TenantId={TenantId} DepartmentId={Id}", TenantId, id);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    /// <summary>True if following <see cref="Department.ParentDepartmentId"/> from <paramref name="startId"/> revisits a node.</summary>
    private async Task<bool> DepartmentParentChainIsCorruptAsync(long startId, CancellationToken cancellationToken)
    {
        var seen = new HashSet<long>();
        long? current = startId;
        for (var depth = 0; depth < 512 && current is not null; depth++)
        {
            if (!seen.Add(current.Value))
                return true;

            current = await _db.Departments.AsNoTracking()
                .Where(d => d.TenantId == TenantId && d.Id == current)
                .Select(d => d.ParentDepartmentId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return false;
    }

    /// <summary>True if walking from <paramref name="startParentId"/> upward eventually reaches <paramref name="blockedDescendantId"/>.</summary>
    private async Task<bool> DepartmentHierarchyWouldCycleAsync(long blockedDescendantId, long startParentId, CancellationToken cancellationToken)
    {
        long? current = startParentId;
        var depth = 0;
        while (current is not null && depth < 512)
        {
            if (current == blockedDescendantId)
                return true;

            current = await _db.Departments.AsNoTracking()
                .Where(d => d.TenantId == TenantId && d.Id == current)
                .Select(d => d.ParentDepartmentId)
                .FirstOrDefaultAsync(cancellationToken);

            depth++;
        }

        return depth >= 512;
    }
}
