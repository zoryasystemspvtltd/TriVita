using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Services.Enterprise;

public sealed class EnterpriseService : IEnterpriseService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly ILogger<EnterpriseService> _logger;

    public EnterpriseService(SharedDbContext db, ITenantContext tenant, ILogger<EnterpriseService> logger)
    {
        _db = db;
        _tenant = tenant;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;

    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<EnterpriseResponseDto>>> ListAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.Enterprises.AsNoTracking()
            .Where(e => e.TenantId == TenantId && !e.IsDeleted)
            .OrderBy(e => e.EnterpriseCode)
            .ToListAsync(cancellationToken);

        var items = rows.Select(e => e.ToDto()).ToList();
        return BaseResponse<IReadOnlyList<EnterpriseResponseDto>>.Ok(items);
    }

    public async Task<BaseResponse<EnterpriseResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Enterprises.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == TenantId && !x.IsDeleted, cancellationToken);

        return e is null
            ? BaseResponse<EnterpriseResponseDto>.Fail("Enterprise not found.")
            : BaseResponse<EnterpriseResponseDto>.Ok(e.ToDto());
    }

    public async Task<BaseResponse<EnterpriseResponseDto>> CreateAsync(CreateEnterpriseDto dto, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var entity = new EnterpriseRoot
        {
            TenantId = TenantId,
            EnterpriseCode = dto.EnterpriseCode.Trim(),
            EnterpriseName = dto.EnterpriseName.Trim(),
            RegistrationDetails = dto.RegistrationDetails,
            GlobalSettingsJson = dto.GlobalSettingsJson,
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

        if (entity.PrimaryAddressId is { } pa &&
            !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<EnterpriseResponseDto>.Fail("Primary address not found for this tenant.");

        if (entity.PrimaryContactId is { } pc &&
            !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<EnterpriseResponseDto>.Fail("Primary contact not found for this tenant.");

        _db.Enterprises.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Enterprise created TenantId={TenantId} EnterpriseId={EnterpriseId} Code={Code}",
            TenantId,
            entity.Id,
            entity.EnterpriseCode);

        return BaseResponse<EnterpriseResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<EnterpriseResponseDto>> UpdateAsync(long id, UpdateEnterpriseDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Enterprises.FirstOrDefaultAsync(
            x => x.Id == id && x.TenantId == TenantId && !x.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<EnterpriseResponseDto>.Fail("Enterprise not found.");

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<EnterpriseResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<EnterpriseResponseDto>.Fail("Primary contact not found for this tenant.");

        entity.EnterpriseCode = dto.EnterpriseCode.Trim();
        entity.EnterpriseName = dto.EnterpriseName.Trim();
        entity.RegistrationDetails = dto.RegistrationDetails;
        entity.GlobalSettingsJson = dto.GlobalSettingsJson;
        entity.PrimaryAddressId = dto.PrimaryAddressId;
        entity.PrimaryContactId = dto.PrimaryContactId;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveTo = dto.EffectiveTo;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Enterprise updated TenantId={TenantId} EnterpriseId={EnterpriseId}", TenantId, id);

        return BaseResponse<EnterpriseResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Enterprises.FirstOrDefaultAsync(
            x => x.Id == id && x.TenantId == TenantId && !x.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Enterprise not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Enterprise soft-deleted TenantId={TenantId} EnterpriseId={EnterpriseId}", TenantId, id);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    public async Task<BaseResponse<EnterpriseHierarchyResponseDto>> GetHierarchyAsync(long enterpriseId, CancellationToken cancellationToken = default)
    {
        var ent = await _db.Enterprises.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == enterpriseId && e.TenantId == TenantId && !e.IsDeleted, cancellationToken);

        if (ent is null)
            return BaseResponse<EnterpriseHierarchyResponseDto>.Fail("Enterprise not found.");

        var companies = await _db.Companies.AsNoTracking()
            .Where(c => c.TenantId == TenantId && !c.IsDeleted && c.EnterpriseId == enterpriseId)
            .OrderBy(c => c.CompanyCode)
            .ToListAsync(cancellationToken);

        var companyIds = companies.Select(c => c.Id).ToList();
        var businessUnits = await _db.BusinessUnits.AsNoTracking()
            .Where(b => b.TenantId == TenantId && !b.IsDeleted && companyIds.Contains(b.CompanyId))
            .OrderBy(b => b.BusinessUnitCode)
            .ToListAsync(cancellationToken);

        var buIds = businessUnits.Select(b => b.Id).ToList();
        var facilities = await _db.Facilities.AsNoTracking()
            .Where(f => f.TenantId == TenantId && !f.IsDeleted && buIds.Contains(f.BusinessUnitId))
            .OrderBy(f => f.FacilityCode)
            .ToListAsync(cancellationToken);

        var facIds = facilities.Select(f => f.Id).ToList();
        var departments = await _db.Departments.AsNoTracking()
            .Where(d => d.TenantId == TenantId && !d.IsDeleted && facIds.Contains(d.FacilityParentId))
            .OrderBy(d => d.DepartmentCode)
            .ToListAsync(cancellationToken);

        var deptByFacility = departments
            .GroupBy(d => d.FacilityParentId)
            .ToDictionary(g => g.Key, g => g.Select(d => d.ToDto()).ToList());

        var facByBu = facilities
            .GroupBy(f => f.BusinessUnitId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var buNodes = businessUnits.Select(bu => new BusinessUnitHierarchyNodeDto
        {
            BusinessUnit = bu.ToDto(),
            Facilities = (facByBu.GetValueOrDefault(bu.Id) ?? new List<Facility>())
                .Select(f => new FacilityHierarchyNodeDto
                {
                    Facility = f.ToDto(),
                    Departments = deptByFacility.GetValueOrDefault(f.Id) ?? new List<DepartmentResponseDto>()
                })
                .ToList()
        }).ToList();

        var buByCompany = buNodes
            .GroupBy(n => n.BusinessUnit.CompanyId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var companyNodes = companies.Select(c => new CompanyHierarchyNodeDto
        {
            Company = c.ToDto(),
            BusinessUnits = buByCompany.GetValueOrDefault(c.Id) ?? new List<BusinessUnitHierarchyNodeDto>()
        }).ToList();

        var tree = new EnterpriseHierarchyResponseDto
        {
            Enterprise = ent.ToDto(),
            Companies = companyNodes
        };

        return BaseResponse<EnterpriseHierarchyResponseDto>.Ok(tree);
    }
}
