using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Application.Services.FeatureExtensions;
using SharedService.Domain.FeatureExtensions;
using SharedService.Infrastructure.Persistence;
using SharedService.Infrastructure.Services.Enterprise;

namespace SharedService.Infrastructure.Services.FeatureExtensions;

public sealed class FacilityServicePriceListService : IFacilityServicePriceListService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IValidator<CreateFacilityServicePriceListDto> _createValidator;
    private readonly IValidator<UpdateFacilityServicePriceListDto> _updateValidator;
    private readonly ILogger<FacilityServicePriceListService> _logger;

    public FacilityServicePriceListService(
        SharedDbContext db,
        ITenantContext tenant,
        IValidator<CreateFacilityServicePriceListDto> createValidator,
        IValidator<UpdateFacilityServicePriceListDto> updateValidator,
        ILogger<FacilityServicePriceListService> logger)
    {
        _db = db;
        _tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;
    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<FacilityServicePriceListResponseDto>>> ListByFacilityAsync(
        long facilityId,
        CancellationToken cancellationToken = default)
    {
        if (!await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, facilityId, cancellationToken))
            return BaseResponse<IReadOnlyList<FacilityServicePriceListResponseDto>>.Fail("Facility not found.");

        var rows = await _db.FacilityServicePriceLists.AsNoTracking()
            .Where(e => e.TenantId == TenantId && e.FacilityId == facilityId && !e.IsDeleted)
            .OrderBy(e => e.PriceListCode)
            .ToListAsync(cancellationToken);

        return BaseResponse<IReadOnlyList<FacilityServicePriceListResponseDto>>.Ok(rows.Select(e => e.ToDto()).ToList());
    }

    public async Task<BaseResponse<FacilityServicePriceListResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var row = await _db.FacilityServicePriceLists.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted, cancellationToken);

        return row is null
            ? BaseResponse<FacilityServicePriceListResponseDto>.Fail("Price list not found.")
            : BaseResponse<FacilityServicePriceListResponseDto>.Ok(row.ToDto());
    }

    public async Task<BaseResponse<FacilityServicePriceListResponseDto>> CreateAsync(
        CreateFacilityServicePriceListDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<FacilityServicePriceListResponseDto>(vr);

        if (!await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, dto.FacilityId, cancellationToken))
            return BaseResponse<FacilityServicePriceListResponseDto>.Fail("Facility not found.");

        var code = dto.PriceListCode.Trim();
        if (await _db.FacilityServicePriceLists.AnyAsync(
                e => e.TenantId == TenantId && e.FacilityId == dto.FacilityId && e.PriceListCode == code && !e.IsDeleted,
                cancellationToken))
            return BaseResponse<FacilityServicePriceListResponseDto>.Fail("Price list code already exists for this facility.");

        var now = DateTime.UtcNow;
        var entity = new FacilityServicePriceList
        {
            TenantId = TenantId,
            FacilityId = dto.FacilityId,
            PriceListCode = code,
            PriceListName = dto.PriceListName.Trim(),
            ServiceModule = dto.ServiceModule.Trim(),
            PartnerReferenceCode = dto.PartnerReferenceCode?.Trim(),
            CurrencyCode = dto.CurrencyCode.Trim(),
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = AuditUser,
            ModifiedBy = AuditUser
        };

        _db.FacilityServicePriceLists.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Price list created TenantId={TenantId} Id={Id} Code={Code}", TenantId, entity.Id, entity.PriceListCode);

        return BaseResponse<FacilityServicePriceListResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<FacilityServicePriceListResponseDto>> UpdateAsync(
        long id,
        UpdateFacilityServicePriceListDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<FacilityServicePriceListResponseDto>(vr);

        var entity = await _db.FacilityServicePriceLists.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<FacilityServicePriceListResponseDto>.Fail("Price list not found.");

        entity.PriceListName = dto.PriceListName.Trim();
        entity.ServiceModule = dto.ServiceModule.Trim();
        entity.PartnerReferenceCode = dto.PartnerReferenceCode?.Trim();
        entity.CurrencyCode = dto.CurrencyCode.Trim();
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveTo = dto.EffectiveTo;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<FacilityServicePriceListResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.FacilityServicePriceLists.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Price list not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    private static BaseResponse<T> FailValidation<T>(ValidationResult vr) =>
        BaseResponse<T>.Fail(string.Join(" ", vr.Errors.Select(e => e.ErrorMessage)));
}
