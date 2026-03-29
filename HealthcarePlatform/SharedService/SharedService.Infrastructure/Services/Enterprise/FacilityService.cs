using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Services.Enterprise;

public sealed class FacilityService : IFacilityService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly ILogger<FacilityService> _logger;

    public FacilityService(SharedDbContext db, ITenantContext tenant, ILogger<FacilityService> logger)
    {
        _db = db;
        _tenant = tenant;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;

    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<FacilityResponseDto>>> ListAsync(long? businessUnitId, CancellationToken cancellationToken = default)
    {
        var q = _db.Facilities.AsNoTracking().Where(f => f.TenantId == TenantId && !f.IsDeleted);
        if (businessUnitId is { } buid)
            q = q.Where(f => f.BusinessUnitId == buid);

        var rows = await q.OrderBy(f => f.FacilityCode).ToListAsync(cancellationToken);
        var items = rows.Select(f => f.ToDto()).ToList();
        return BaseResponse<IReadOnlyList<FacilityResponseDto>>.Ok(items);
    }

    public async Task<BaseResponse<FacilityResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Facilities.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id && f.TenantId == TenantId && !f.IsDeleted, cancellationToken);

        return e is null
            ? BaseResponse<FacilityResponseDto>.Fail("Facility not found.")
            : BaseResponse<FacilityResponseDto>.Ok(e.ToDto());
    }

    public async Task<BaseResponse<FacilityHierarchyContextDto>> GetHierarchyContextAsync(long facilityId, CancellationToken cancellationToken = default)
    {
        var facility = await _db.Facilities.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == facilityId && f.TenantId == TenantId && !f.IsDeleted, cancellationToken);

        if (facility is null)
        {
            _logger.LogWarning(
                "Facility hierarchy context miss TenantId={TenantId} FacilityId={FacilityId}",
                TenantId,
                facilityId);
            return BaseResponse<FacilityHierarchyContextDto>.Fail("Facility not found for this tenant.");
        }

        var bu = await _db.BusinessUnits.AsNoTracking()
            .FirstOrDefaultAsync(
                b => b.Id == facility.BusinessUnitId && b.TenantId == TenantId && !b.IsDeleted,
                cancellationToken);

        if (bu is null)
            return BaseResponse<FacilityHierarchyContextDto>.Fail("Business unit chain is invalid (missing business unit).");

        var company = await _db.Companies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == bu.CompanyId && c.TenantId == TenantId && !c.IsDeleted, cancellationToken);

        if (company is null)
            return BaseResponse<FacilityHierarchyContextDto>.Fail("Company chain is invalid (missing company).");

        var enterprise = await _db.Enterprises.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == company.EnterpriseId && e.TenantId == TenantId && !e.IsDeleted, cancellationToken);

        if (enterprise is null)
            return BaseResponse<FacilityHierarchyContextDto>.Fail("Enterprise chain is invalid (missing enterprise).");

        var dto = new FacilityHierarchyContextDto
        {
            TenantId = TenantId,
            EnterpriseId = enterprise.Id,
            CompanyId = company.Id,
            BusinessUnitId = bu.Id,
            FacilityId = facility.Id,
            FacilityCode = facility.FacilityCode,
            FacilityName = facility.FacilityName,
            BusinessUnitType = bu.BusinessUnitType,
            FacilityType = facility.FacilityType
        };

        return BaseResponse<FacilityHierarchyContextDto>.Ok(dto);
    }

    public async Task<BaseResponse<FacilityResponseDto>> CreateAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        if (!await EnterpriseReferenceGuard.BusinessUnitExistsAsync(_db, TenantId, dto.BusinessUnitId, cancellationToken))
            return BaseResponse<FacilityResponseDto>.Fail("Business unit not found for this tenant (parent constraint).");

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<FacilityResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<FacilityResponseDto>.Fail("Primary contact not found for this tenant.");

        var now = DateTime.UtcNow;
        var entity = new Facility
        {
            TenantId = TenantId,
            BusinessUnitId = dto.BusinessUnitId,
            FacilityCode = dto.FacilityCode.Trim(),
            FacilityName = dto.FacilityName.Trim(),
            FacilityType = dto.FacilityType.Trim(),
            LicenseDetails = dto.LicenseDetails,
            TimeZoneId = dto.TimeZoneId,
            GeoCode = dto.GeoCode,
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

        _db.Facilities.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Facility created TenantId={TenantId} FacilityId={Id} Code={Code}",
            TenantId,
            entity.Id,
            entity.FacilityCode);

        return BaseResponse<FacilityResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<FacilityResponseDto>> UpdateAsync(long id, UpdateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Facilities.FirstOrDefaultAsync(
            f => f.Id == id && f.TenantId == TenantId && !f.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<FacilityResponseDto>.Fail("Facility not found.");

        if (!await EnterpriseReferenceGuard.BusinessUnitExistsAsync(_db, TenantId, dto.BusinessUnitId, cancellationToken))
            return BaseResponse<FacilityResponseDto>.Fail("Business unit not found for this tenant (parent constraint).");

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<FacilityResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<FacilityResponseDto>.Fail("Primary contact not found for this tenant.");

        entity.BusinessUnitId = dto.BusinessUnitId;
        entity.FacilityCode = dto.FacilityCode.Trim();
        entity.FacilityName = dto.FacilityName.Trim();
        entity.FacilityType = dto.FacilityType.Trim();
        entity.LicenseDetails = dto.LicenseDetails;
        entity.TimeZoneId = dto.TimeZoneId;
        entity.GeoCode = dto.GeoCode;
        entity.PrimaryAddressId = dto.PrimaryAddressId;
        entity.PrimaryContactId = dto.PrimaryContactId;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveTo = dto.EffectiveTo;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility updated TenantId={TenantId} FacilityId={Id}", TenantId, id);

        return BaseResponse<FacilityResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Facilities.FirstOrDefaultAsync(
            f => f.Id == id && f.TenantId == TenantId && !f.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Facility not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facility soft-deleted TenantId={TenantId} FacilityId={Id}", TenantId, id);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}
