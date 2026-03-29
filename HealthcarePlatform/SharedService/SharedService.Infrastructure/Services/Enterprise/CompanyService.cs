using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Services.Enterprise;

public sealed class CompanyService : ICompanyService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(SharedDbContext db, ITenantContext tenant, ILogger<CompanyService> logger)
    {
        _db = db;
        _tenant = tenant;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;

    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<CompanyResponseDto>>> ListAsync(long? enterpriseId, CancellationToken cancellationToken = default)
    {
        var q = _db.Companies.AsNoTracking().Where(c => c.TenantId == TenantId && !c.IsDeleted);
        if (enterpriseId is { } eid)
            q = q.Where(c => c.EnterpriseId == eid);

        var rows = await q.OrderBy(c => c.CompanyCode).ToListAsync(cancellationToken);
        var items = rows.Select(c => c.ToDto()).ToList();
        return BaseResponse<IReadOnlyList<CompanyResponseDto>>.Ok(items);
    }

    public async Task<BaseResponse<CompanyResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var e = await _db.Companies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == TenantId && !c.IsDeleted, cancellationToken);

        return e is null
            ? BaseResponse<CompanyResponseDto>.Fail("Company not found.")
            : BaseResponse<CompanyResponseDto>.Ok(e.ToDto());
    }

    public async Task<BaseResponse<CompanyResponseDto>> CreateAsync(CreateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        if (!await EnterpriseReferenceGuard.EnterpriseExistsAsync(_db, TenantId, dto.EnterpriseId, cancellationToken))
            return BaseResponse<CompanyResponseDto>.Fail("Enterprise not found for this tenant (parent constraint).");

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<CompanyResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<CompanyResponseDto>.Fail("Primary contact not found for this tenant.");

        var now = DateTime.UtcNow;
        var entity = new Company
        {
            TenantId = TenantId,
            EnterpriseId = dto.EnterpriseId,
            CompanyCode = dto.CompanyCode.Trim(),
            CompanyName = dto.CompanyName.Trim(),
            PAN = dto.PAN,
            GSTIN = dto.GSTIN,
            LegalIdentifier1 = dto.LegalIdentifier1,
            LegalIdentifier2 = dto.LegalIdentifier2,
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

        _db.Companies.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Company created TenantId={TenantId} CompanyId={CompanyId} Code={Code}",
            TenantId,
            entity.Id,
            entity.CompanyCode);

        return BaseResponse<CompanyResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<CompanyResponseDto>> UpdateAsync(long id, UpdateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Companies.FirstOrDefaultAsync(
            c => c.Id == id && c.TenantId == TenantId && !c.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<CompanyResponseDto>.Fail("Company not found.");

        if (!await EnterpriseReferenceGuard.EnterpriseExistsAsync(_db, TenantId, dto.EnterpriseId, cancellationToken))
            return BaseResponse<CompanyResponseDto>.Fail("Enterprise not found for this tenant (parent constraint).");

        if (dto.PrimaryAddressId is { } pa && !await EnterpriseReferenceGuard.AddressExistsAsync(_db, TenantId, pa, cancellationToken))
            return BaseResponse<CompanyResponseDto>.Fail("Primary address not found for this tenant.");

        if (dto.PrimaryContactId is { } pc && !await EnterpriseReferenceGuard.ContactExistsAsync(_db, TenantId, pc, cancellationToken))
            return BaseResponse<CompanyResponseDto>.Fail("Primary contact not found for this tenant.");

        entity.EnterpriseId = dto.EnterpriseId;
        entity.CompanyCode = dto.CompanyCode.Trim();
        entity.CompanyName = dto.CompanyName.Trim();
        entity.PAN = dto.PAN;
        entity.GSTIN = dto.GSTIN;
        entity.LegalIdentifier1 = dto.LegalIdentifier1;
        entity.LegalIdentifier2 = dto.LegalIdentifier2;
        entity.PrimaryAddressId = dto.PrimaryAddressId;
        entity.PrimaryContactId = dto.PrimaryContactId;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveTo = dto.EffectiveTo;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Company updated TenantId={TenantId} CompanyId={CompanyId}", TenantId, id);

        return BaseResponse<CompanyResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Companies.FirstOrDefaultAsync(
            c => c.Id == id && c.TenantId == TenantId && !c.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Company not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Company soft-deleted TenantId={TenantId} CompanyId={CompanyId}", TenantId, id);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}
