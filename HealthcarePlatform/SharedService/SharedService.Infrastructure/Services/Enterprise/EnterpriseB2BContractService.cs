using FluentValidation;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;

namespace SharedService.Infrastructure.Services.Enterprise;

public sealed class EnterpriseB2BContractService : IEnterpriseB2BContractService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IValidator<CreateEnterpriseB2BContractDto> _createValidator;
    private readonly IValidator<UpdateEnterpriseB2BContractDto> _updateValidator;
    private readonly ILogger<EnterpriseB2BContractService> _logger;

    public EnterpriseB2BContractService(
        SharedDbContext db,
        ITenantContext tenant,
        IValidator<CreateEnterpriseB2BContractDto> createValidator,
        IValidator<UpdateEnterpriseB2BContractDto> updateValidator,
        ILogger<EnterpriseB2BContractService> logger)
    {
        _db = db;
        _tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;

    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<EnterpriseB2BContractResponseDto>>> ListByEnterpriseAsync(
        long enterpriseId,
        CancellationToken cancellationToken = default)
    {
        if (!await EnterpriseReferenceGuard.EnterpriseExistsAsync(_db, TenantId, enterpriseId, cancellationToken))
            return BaseResponse<IReadOnlyList<EnterpriseB2BContractResponseDto>>.Fail("Enterprise not found.");

        var rows = await _db.EnterpriseB2BContracts.AsNoTracking()
            .Where(c => c.TenantId == TenantId && c.EnterpriseId == enterpriseId && !c.IsDeleted)
            .OrderBy(c => c.ContractCode)
            .ToListAsync(cancellationToken);

        return BaseResponse<IReadOnlyList<EnterpriseB2BContractResponseDto>>.Ok(
            rows.Select(c => c.ToDto()).ToList());
    }

    public async Task<BaseResponse<EnterpriseB2BContractResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var row = await _db.EnterpriseB2BContracts.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == TenantId && !c.IsDeleted, cancellationToken);

        return row is null
            ? BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Contract not found.")
            : BaseResponse<EnterpriseB2BContractResponseDto>.Ok(row.ToDto());
    }

    public async Task<BaseResponse<EnterpriseB2BContractResponseDto>> CreateAsync(
        CreateEnterpriseB2BContractDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<EnterpriseB2BContractResponseDto>.Fail(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        if (!await EnterpriseReferenceGuard.EnterpriseExistsAsync(_db, TenantId, dto.EnterpriseId, cancellationToken))
            return BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Enterprise not found.");

        if (dto.FacilityId is { } fid &&
            !await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, fid, cancellationToken))
            return BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Facility not found for this tenant.");

        var code = dto.ContractCode.Trim();
        var duplicate = await _db.EnterpriseB2BContracts.AnyAsync(
            c => c.TenantId == TenantId && c.EnterpriseId == dto.EnterpriseId && c.ContractCode == code && !c.IsDeleted,
            cancellationToken);
        if (duplicate)
            return BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Contract code already exists for this enterprise.");

        var now = DateTime.UtcNow;
        var entity = new EnterpriseB2BContract
        {
            TenantId = TenantId,
            EnterpriseId = dto.EnterpriseId,
            FacilityId = dto.FacilityId,
            PartnerType = dto.PartnerType.Trim(),
            PartnerName = dto.PartnerName.Trim(),
            ContractCode = code,
            TermsJson = dto.TermsJson,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = AuditUser,
            ModifiedBy = AuditUser
        };

        _db.EnterpriseB2BContracts.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "B2B contract created TenantId={TenantId} ContractId={Id} EnterpriseId={EnterpriseId} Code={Code}",
            TenantId,
            entity.Id,
            entity.EnterpriseId,
            entity.ContractCode);

        return BaseResponse<EnterpriseB2BContractResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<EnterpriseB2BContractResponseDto>> UpdateAsync(
        long id,
        UpdateEnterpriseB2BContractDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<EnterpriseB2BContractResponseDto>.Fail(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var entity = await _db.EnterpriseB2BContracts.FirstOrDefaultAsync(
            c => c.Id == id && c.TenantId == TenantId && !c.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Contract not found.");

        if (dto.FacilityId is { } fid &&
            !await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, fid, cancellationToken))
            return BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Facility not found for this tenant.");

        var code = dto.ContractCode.Trim();
        var duplicate = await _db.EnterpriseB2BContracts.AnyAsync(
            c => c.TenantId == TenantId && c.EnterpriseId == entity.EnterpriseId && c.ContractCode == code && c.Id != id && !c.IsDeleted,
            cancellationToken);
        if (duplicate)
            return BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Contract code already exists for this enterprise.");

        entity.FacilityId = dto.FacilityId;
        entity.PartnerType = dto.PartnerType.Trim();
        entity.PartnerName = dto.PartnerName.Trim();
        entity.ContractCode = code;
        entity.TermsJson = dto.TermsJson;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveTo = dto.EffectiveTo;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<EnterpriseB2BContractResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.EnterpriseB2BContracts.FirstOrDefaultAsync(
            c => c.Id == id && c.TenantId == TenantId && !c.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Contract not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}
