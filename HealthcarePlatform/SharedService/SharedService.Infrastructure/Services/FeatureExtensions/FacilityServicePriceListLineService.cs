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

public sealed class FacilityServicePriceListLineService : IFacilityServicePriceListLineService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IValidator<CreateFacilityServicePriceListLineDto> _createValidator;
    private readonly IValidator<UpdateFacilityServicePriceListLineDto> _updateValidator;
    private readonly ILogger<FacilityServicePriceListLineService> _logger;

    public FacilityServicePriceListLineService(
        SharedDbContext db,
        ITenantContext tenant,
        IValidator<CreateFacilityServicePriceListLineDto> createValidator,
        IValidator<UpdateFacilityServicePriceListLineDto> updateValidator,
        ILogger<FacilityServicePriceListLineService> logger)
    {
        _db = db;
        _tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;
    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<FacilityServicePriceListLineResponseDto>>> ListByPriceListAsync(
        long facilityId,
        long priceListId,
        CancellationToken cancellationToken = default)
    {
        if (!await PriceListExistsAsync(facilityId, priceListId, cancellationToken))
            return BaseResponse<IReadOnlyList<FacilityServicePriceListLineResponseDto>>.Fail("Price list not found.");

        var rows = await _db.FacilityServicePriceListLines.AsNoTracking()
            .Where(e => e.TenantId == TenantId && e.FacilityId == facilityId && e.PriceListId == priceListId && !e.IsDeleted)
            .OrderBy(e => e.ServiceItemCode)
            .ToListAsync(cancellationToken);

        return BaseResponse<IReadOnlyList<FacilityServicePriceListLineResponseDto>>.Ok(rows.Select(e => e.ToDto()).ToList());
    }

    public async Task<BaseResponse<FacilityServicePriceListLineResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var row = await _db.FacilityServicePriceListLines.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted, cancellationToken);

        return row is null
            ? BaseResponse<FacilityServicePriceListLineResponseDto>.Fail("Price list line not found.")
            : BaseResponse<FacilityServicePriceListLineResponseDto>.Ok(row.ToDto());
    }

    public async Task<BaseResponse<FacilityServicePriceListLineResponseDto>> CreateAsync(
        CreateFacilityServicePriceListLineDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<FacilityServicePriceListLineResponseDto>(vr);

        if (!await PriceListExistsAsync(dto.FacilityId, dto.PriceListId, cancellationToken))
            return BaseResponse<FacilityServicePriceListLineResponseDto>.Fail("Price list not found.");

        var code = dto.ServiceItemCode.Trim();
        if (await _db.FacilityServicePriceListLines.AnyAsync(
                e => e.TenantId == TenantId && e.FacilityId == dto.FacilityId && e.PriceListId == dto.PriceListId &&
                     e.ServiceItemCode == code && !e.IsDeleted,
                cancellationToken))
            return BaseResponse<FacilityServicePriceListLineResponseDto>.Fail("Service item code already exists on this price list.");

        var now = DateTime.UtcNow;
        var entity = new FacilityServicePriceListLine
        {
            TenantId = TenantId,
            FacilityId = dto.FacilityId,
            PriceListId = dto.PriceListId,
            ServiceItemCode = code,
            ServiceItemName = dto.ServiceItemName?.Trim(),
            UnitPrice = dto.UnitPrice,
            TaxCategoryCode = dto.TaxCategoryCode?.Trim(),
            IsActive = true,
            IsDeleted = false,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = AuditUser,
            ModifiedBy = AuditUser
        };

        _db.FacilityServicePriceListLines.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Price list line created TenantId={TenantId} Id={Id}", TenantId, entity.Id);

        return BaseResponse<FacilityServicePriceListLineResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<FacilityServicePriceListLineResponseDto>> UpdateAsync(
        long id,
        UpdateFacilityServicePriceListLineDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<FacilityServicePriceListLineResponseDto>(vr);

        var entity = await _db.FacilityServicePriceListLines.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<FacilityServicePriceListLineResponseDto>.Fail("Price list line not found.");

        entity.ServiceItemName = dto.ServiceItemName?.Trim();
        entity.UnitPrice = dto.UnitPrice;
        entity.TaxCategoryCode = dto.TaxCategoryCode?.Trim();
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<FacilityServicePriceListLineResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.FacilityServicePriceListLines.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Price list line not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    private Task<bool> PriceListExistsAsync(long facilityId, long priceListId, CancellationToken ct) =>
        _db.FacilityServicePriceLists.AsNoTracking().AnyAsync(
            e => e.Id == priceListId && e.TenantId == TenantId && e.FacilityId == facilityId && !e.IsDeleted,
            ct);

    private static BaseResponse<T> FailValidation<T>(ValidationResult vr) =>
        BaseResponse<T>.Fail(string.Join(" ", vr.Errors.Select(e => e.ErrorMessage)));
}
