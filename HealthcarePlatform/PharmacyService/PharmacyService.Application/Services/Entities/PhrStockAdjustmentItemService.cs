using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Extended;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrStockAdjustmentItemService
{
    Task<BaseResponse<StockAdjustmentItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockAdjustmentItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IReadOnlyList<StockAdjustmentItemResponseDto>>> GetByStockAdjustmentIdAsync(long stockAdjustmentId, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentItemResponseDto>> CreateAsync(CreateStockAdjustmentItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentItemResponseDto>> UpdateAsync(long id, UpdateStockAdjustmentItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockAdjustmentItemService : PhrCrudServiceBase<PhrStockAdjustmentItem, CreateStockAdjustmentItemDto, UpdateStockAdjustmentItemDto, StockAdjustmentItemResponseDto, PhrStockAdjustmentItemService>, IPhrStockAdjustmentItemService
{
    private readonly IRepository<PhrStockAdjustment> _adjustments;
    private readonly IRepository<PhrMedicineBatch> _batches;
    private readonly IRepository<PhrMedicine> _medicines;
    private readonly IRepository<PhrBatchStock> _batchStocks;

    public PhrStockAdjustmentItemService(
        IRepository<PhrStockAdjustmentItem> repository,
        IRepository<PhrStockAdjustment> adjustments,
        IRepository<PhrMedicineBatch> batches,
        IRepository<PhrMedicine> medicines,
        IRepository<PhrBatchStock> batchStocks,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateStockAdjustmentItemDto>? createValidator,
        IValidator<UpdateStockAdjustmentItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrStockAdjustmentItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _adjustments = adjustments;
        _batches = batches;
        _medicines = medicines;
        _batchStocks = batchStocks;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockAdjustmentItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public async Task<BaseResponse<IReadOnlyList<StockAdjustmentItemResponseDto>>> GetByStockAdjustmentIdAsync(
        long stockAdjustmentId,
        CancellationToken cancellationToken = default)
    {
        var adj = await _adjustments.GetByIdAsync(stockAdjustmentId, cancellationToken);
        if (adj is null)
            return BaseResponse<IReadOnlyList<StockAdjustmentItemResponseDto>>.Fail("Stock adjustment not found.");
        if (Tenant.FacilityId is { } tf && adj.FacilityId is { } af && af != tf)
            return BaseResponse<IReadOnlyList<StockAdjustmentItemResponseDto>>.Fail("Resource is not in the current facility scope.");

        var lines = await Repository.ListAsync(
            i => i.StockAdjustmentId == stockAdjustmentId && !i.IsDeleted,
            cancellationToken);
        var ordered = lines.OrderBy(i => i.LineNum).ThenBy(i => i.Id).ToList();
        var list = new List<StockAdjustmentItemResponseDto>();
        foreach (var line in ordered)
        {
            var dto = Mapper.Map<StockAdjustmentItemResponseDto>(line);
            await EnrichLineAsync(dto, cancellationToken);
            list.Add(dto);
        }

        return BaseResponse<IReadOnlyList<StockAdjustmentItemResponseDto>>.Ok(list);
    }

    public override async Task<BaseResponse<StockAdjustmentItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var r = await base.GetByIdAsync(id, cancellationToken);
        if (!r.Success || r.Data is null)
            return r;
        await EnrichLineAsync(r.Data, cancellationToken);
        return r;
    }

    public override async Task<BaseResponse<StockAdjustmentItemResponseDto>> CreateAsync(
        CreateStockAdjustmentItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantityDelta == 0)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("QuantityDelta must be non-zero.");
        if (dto.MedicineId <= 0)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine is required.");
        if (dto.MedicineBatchId <= 0)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Batch is required.");

        var adj = await _adjustments.GetByIdAsync(dto.StockAdjustmentId, cancellationToken);
        if (adj is null) return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Stock adjustment not found.");
        if (Tenant.FacilityId is { } tf && adj.FacilityId is { } af && af != tf)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Resource is not in the current facility scope.");
        if (adj.Status != PharmacyStockAdjustmentStatus.Draft)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Lines cannot be added to a posted adjustment.");

        var batch = await _batches.GetByIdAsync(dto.MedicineBatchId, cancellationToken);
        if (batch is null || batch.IsDeleted)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine batch not found.");
        if (batch.MedicineId != dto.MedicineId)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine does not match the selected batch.");

        var entity = Mapper.Map<PhrStockAdjustmentItem>(dto);
        if (entity.LineNum <= 0)
            entity.LineNum = await NextLineNumAsync(dto.StockAdjustmentId, cancellationToken);

        AuditHelper.ApplyCreate(entity, Tenant);
        entity.FacilityId = Tenant.FacilityId;

        await Repository.AddAsync(entity, cancellationToken);
        await Repository.SaveChangesAsync(cancellationToken);

        var response = Mapper.Map<StockAdjustmentItemResponseDto>(entity);
        await EnrichLineAsync(response, cancellationToken);
        return BaseResponse<StockAdjustmentItemResponseDto>.Ok(response, "Created.");
    }

    public override async Task<BaseResponse<StockAdjustmentItemResponseDto>> UpdateAsync(
        long id,
        UpdateStockAdjustmentItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantityDelta == 0)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("QuantityDelta must be non-zero.");
        if (dto.MedicineId <= 0)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine is required.");
        if (dto.MedicineBatchId <= 0)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Batch is required.");

        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<StockAdjustmentItemResponseDto>.Fail("StockAdjustmentItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Resource is not in the current facility scope.");

        var adj = await _adjustments.GetByIdAsync(entity.StockAdjustmentId, cancellationToken);
        if (adj is null) return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Stock adjustment not found.");
        if (adj.Status != PharmacyStockAdjustmentStatus.Draft)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Posted adjustments are locked.");

        var batch = await _batches.GetByIdAsync(dto.MedicineBatchId, cancellationToken);
        if (batch is null || batch.IsDeleted)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine batch not found.");
        if (batch.MedicineId != dto.MedicineId)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine does not match the selected batch.");

        Mapper.Map(dto, entity);
        AuditHelper.ApplyUpdate(entity, Tenant);
        await Repository.UpdateAsync(entity, cancellationToken);
        await Repository.SaveChangesAsync(cancellationToken);

        var response = Mapper.Map<StockAdjustmentItemResponseDto>(entity);
        await EnrichLineAsync(response, cancellationToken);
        return BaseResponse<StockAdjustmentItemResponseDto>.Ok(response, "Updated.");
    }

    public override async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<object?>.Fail("StockAdjustmentItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        var adj = await _adjustments.GetByIdAsync(entity.StockAdjustmentId, cancellationToken);
        if (adj is null) return BaseResponse<object?>.Fail("Stock adjustment not found.");
        if (adj.Status != PharmacyStockAdjustmentStatus.Draft)
            return BaseResponse<object?>.Fail("Posted adjustments are locked.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, Tenant);
        await Repository.UpdateAsync(entity, cancellationToken);
        await Repository.SaveChangesAsync(cancellationToken);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    private async Task EnrichLineAsync(StockAdjustmentItemResponseDto dto, CancellationToken cancellationToken)
    {
        var batch = await _batches.GetByIdAsync(dto.MedicineBatchId, cancellationToken);
        if (batch is null)
            return;

        dto.MedicineId = batch.MedicineId;
        dto.BatchNo = batch.BatchNo;
        dto.ExpiryDate = batch.ExpiryDate;

        var med = await _medicines.GetByIdAsync(batch.MedicineId, cancellationToken);
        dto.MedicineName = med?.MedicineName;

        if (Tenant.FacilityId is { } fid)
        {
            var stocks = await _batchStocks.ListAsync(
                s => s.MedicineBatchId == dto.MedicineBatchId && s.FacilityId == fid && !s.IsDeleted,
                cancellationToken);
            dto.CurrentQty = stocks.FirstOrDefault()?.CurrentQty ?? 0m;
        }
    }

    private async Task<int> NextLineNumAsync(long stockAdjustmentId, CancellationToken cancellationToken)
    {
        var lines = await Repository.ListAsync(i => i.StockAdjustmentId == stockAdjustmentId && !i.IsDeleted, cancellationToken);
        return lines.Count == 0 ? 1 : lines.Max(i => i.LineNum) + 1;
    }
}
