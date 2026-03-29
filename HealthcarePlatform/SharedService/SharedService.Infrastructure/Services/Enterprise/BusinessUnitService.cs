using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Services.Enterprise;

public sealed class BusinessUnitService : IBusinessUnitService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly ILogger<BusinessUnitService> _logger;

    public BusinessUnitService(SharedDbContext db, ITenantContext tenant, ILogger<BusinessUnitService> logger)
    {
        _db = db;
        _tenant = tenant;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;

    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<BusinessUnitResponseDto>>> ListAsync(long? companyId, CancellationToken cancellationToken = default)
    {
        var q = _db.BusinessUnits.AsNoTracking().Where(b => b.TenantId == TenantId && !b.IsDeleted);
        if (companyId is { } cid)
            q = q.Where(b => b.CompanyId == cid);

        var rows = await q.OrderBy(b => b.BusinessUnitCode).ToListAsync(cancellationToken);
        var items = rows.Select(b => b.ToDto()).ToList();
        return BaseResponse<IReadOnlyList<BusinessUnitResponseDto>>.Ok(items);
    }

    public async Task<BaseResponse<BusinessUnitResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var e = await _db.BusinessUnits.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && b.TenantId == TenantId && !b.IsDeleted, cancellationToken);

        return e is null
            ? BaseResponse<BusinessUnitResponseDto>.Fail("Business unit not found.")
            : BaseResponse<BusinessUnitResponseDto>.Ok(e.ToDto());
    }

    public async Task<BaseResponse<BusinessUnitResponseDto>> CreateAsync(CreateBusinessUnitDto dto, CancellationToken cancellationToken = default)
    {
        if (!await EnterpriseReferenceGuard.CompanyExistsAsync(_db, TenantId, dto.CompanyId, cancellationToken))
            return BaseResponse<BusinessUnitResponseDto>.Fail("Company not found for this tenant (parent constraint).");

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<BusinessUnitResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<BusinessUnitResponseDto>.Fail("Primary contact not found for this tenant.");

        var now = DateTime.UtcNow;
        var entity = new BusinessUnit
        {
            TenantId = TenantId,
            CompanyId = dto.CompanyId,
            BusinessUnitCode = dto.BusinessUnitCode.Trim(),
            BusinessUnitName = dto.BusinessUnitName.Trim(),
            BusinessUnitType = dto.BusinessUnitType.Trim(),
            RegionCode = dto.RegionCode,
            CountryCode = dto.CountryCode,
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

        _db.BusinessUnits.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "BusinessUnit created TenantId={TenantId} BusinessUnitId={Id} Code={Code}",
            TenantId,
            entity.Id,
            entity.BusinessUnitCode);

        return BaseResponse<BusinessUnitResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<BusinessUnitResponseDto>> UpdateAsync(long id, UpdateBusinessUnitDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.BusinessUnits.FirstOrDefaultAsync(
            b => b.Id == id && b.TenantId == TenantId && !b.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<BusinessUnitResponseDto>.Fail("Business unit not found.");

        if (!await EnterpriseReferenceGuard.CompanyExistsAsync(_db, TenantId, dto.CompanyId, cancellationToken))
            return BaseResponse<BusinessUnitResponseDto>.Fail("Company not found for this tenant (parent constraint).");

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<BusinessUnitResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<BusinessUnitResponseDto>.Fail("Primary contact not found for this tenant.");

        entity.CompanyId = dto.CompanyId;
        entity.BusinessUnitCode = dto.BusinessUnitCode.Trim();
        entity.BusinessUnitName = dto.BusinessUnitName.Trim();
        entity.BusinessUnitType = dto.BusinessUnitType.Trim();
        entity.RegionCode = dto.RegionCode;
        entity.CountryCode = dto.CountryCode;
        entity.PrimaryAddressId = dto.PrimaryAddressId;
        entity.PrimaryContactId = dto.PrimaryContactId;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveTo = dto.EffectiveTo;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("BusinessUnit updated TenantId={TenantId} BusinessUnitId={Id}", TenantId, id);

        return BaseResponse<BusinessUnitResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.BusinessUnits.FirstOrDefaultAsync(
            b => b.Id == id && b.TenantId == TenantId && !b.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Business unit not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("BusinessUnit soft-deleted TenantId={TenantId} BusinessUnitId={Id}", TenantId, id);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}
