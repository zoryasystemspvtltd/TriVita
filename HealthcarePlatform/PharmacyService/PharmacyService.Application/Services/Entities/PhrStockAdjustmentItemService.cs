using System.Data;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services;
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
    Task<BaseResponse<StockAdjustmentItemResponseDto>> CreateAsync(CreateStockAdjustmentItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentItemResponseDto>> UpdateAsync(long id, UpdateStockAdjustmentItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockAdjustmentItemService : PhrCrudServiceBase<PhrStockAdjustmentItem, CreateStockAdjustmentItemDto, UpdateStockAdjustmentItemDto, StockAdjustmentItemResponseDto, PhrStockAdjustmentItemService>, IPhrStockAdjustmentItemService
{
    private readonly IRepository<PhrStockAdjustment> _adjustments;
    private readonly IRepository<PhrMedicineBatch> _batches;
    private readonly IPharmacyStockMovementService _stockMovement;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrStockAdjustmentItemService(
        IRepository<PhrStockAdjustmentItem> repository,
        IRepository<PhrStockAdjustment> adjustments,
        IRepository<PhrMedicineBatch> batches,
        IPharmacyStockMovementService stockMovement,
        IPharmacyUnitOfWork uow,
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
        _stockMovement = stockMovement;
        _uow = uow;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockAdjustmentItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<StockAdjustmentItemResponseDto>> CreateAsync(
        CreateStockAdjustmentItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantityDelta == 0)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("QuantityDelta must be non-zero.");

        var adj = await _adjustments.GetByIdAsync(dto.StockAdjustmentId, cancellationToken);
        if (adj is null) return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Stock adjustment not found.");
        if (Tenant.FacilityId is { } tf && adj.FacilityId is { } af && af != tf)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Resource is not in the current facility scope.");

        var batch = await _batches.GetByIdAsync(dto.MedicineBatchId, cancellationToken);
        if (batch is null || batch.IsDeleted)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine batch not found.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var entity = Mapper.Map<PhrStockAdjustmentItem>(dto);
                AuditHelper.ApplyCreate(entity, Tenant);
                entity.FacilityId = Tenant.FacilityId;

                await Repository.AddAsync(entity, ct);
                await Repository.SaveChangesAsync(ct);

                var txnDate = adj.AdjustmentOn == default ? DateTime.UtcNow : adj.AdjustmentOn;
                var mv = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.ADJUSTMENT,
                    dto.StockAdjustmentId,
                    entity.Id,
                    batch.MedicineId,
                    dto.MedicineBatchId,
                    dto.QuantityDelta,
                    txnDate,
                    dto.UnitCost,
                    dto.Notes,
                    ct);
                if (!mv.Success)
                    return BaseResponse<StockAdjustmentItemResponseDto>.Fail(mv.Message ?? "Stock adjustment movement failed.");

                return BaseResponse<StockAdjustmentItemResponseDto>.Ok(Mapper.Map<StockAdjustmentItemResponseDto>(entity), "Created.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stock adjustment item create failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Create failed.");
        }
    }

    public override async Task<BaseResponse<StockAdjustmentItemResponseDto>> UpdateAsync(
        long id,
        UpdateStockAdjustmentItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantityDelta == 0)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("QuantityDelta must be non-zero.");

        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<StockAdjustmentItemResponseDto>.Fail("StockAdjustmentItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Resource is not in the current facility scope.");

        var adj = await _adjustments.GetByIdAsync(entity.StockAdjustmentId, cancellationToken);
        if (adj is null) return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Stock adjustment not found.");

        var batch = await _batches.GetByIdAsync(dto.MedicineBatchId, cancellationToken);
        if (batch is null || batch.IsDeleted)
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine batch not found.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var txnDate = adj.AdjustmentOn == default ? DateTime.UtcNow : adj.AdjustmentOn;

                var priorBatch = await _batches.GetByIdAsync(entity.MedicineBatchId, ct);
                if (priorBatch is null)
                    return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine batch not found.");

                var rev = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.ADJUSTMENT,
                    entity.StockAdjustmentId,
                    entity.Id,
                    priorBatch.MedicineId,
                    entity.MedicineBatchId,
                    -entity.QuantityDelta,
                    txnDate,
                    entity.UnitCost,
                    "Adjust line update (reverse)",
                    ct);
                if (!rev.Success)
                    return BaseResponse<StockAdjustmentItemResponseDto>.Fail(rev.Message ?? "Stock reversal failed.");

                Mapper.Map(dto, entity);
                AuditHelper.ApplyUpdate(entity, Tenant);
                await Repository.UpdateAsync(entity, ct);
                await Repository.SaveChangesAsync(ct);

                var newBatch = await _batches.GetByIdAsync(entity.MedicineBatchId, ct);
                if (newBatch is null)
                    return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Medicine batch not found.");

                var mv = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.ADJUSTMENT,
                    entity.StockAdjustmentId,
                    entity.Id,
                    newBatch.MedicineId,
                    entity.MedicineBatchId,
                    dto.QuantityDelta,
                    txnDate,
                    dto.UnitCost,
                    dto.Notes,
                    ct);
                if (!mv.Success)
                    return BaseResponse<StockAdjustmentItemResponseDto>.Fail(mv.Message ?? "Stock adjustment movement failed.");

                return BaseResponse<StockAdjustmentItemResponseDto>.Ok(Mapper.Map<StockAdjustmentItemResponseDto>(entity), "Updated.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stock adjustment item update failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<StockAdjustmentItemResponseDto>.Fail("Update failed.");
        }
    }

    public override async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<object?>.Fail("StockAdjustmentItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        var adj = await _adjustments.GetByIdAsync(entity.StockAdjustmentId, cancellationToken);
        if (adj is null) return BaseResponse<object?>.Fail("Stock adjustment not found.");

        var batch = await _batches.GetByIdAsync(entity.MedicineBatchId, cancellationToken);
        if (batch is null)
            return BaseResponse<object?>.Fail("Medicine batch not found.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var txnDate = adj.AdjustmentOn == default ? DateTime.UtcNow : adj.AdjustmentOn;

                var rev = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.ADJUSTMENT,
                    entity.StockAdjustmentId,
                    entity.Id,
                    batch.MedicineId,
                    entity.MedicineBatchId,
                    -entity.QuantityDelta,
                    txnDate,
                    entity.UnitCost,
                    "Adjust line deleted",
                    ct);
                if (!rev.Success)
                    return BaseResponse<object?>.Fail(rev.Message ?? "Stock reversal failed.");

                entity.IsDeleted = true;
                entity.IsActive = false;
                AuditHelper.ApplyUpdate(entity, Tenant);
                await Repository.UpdateAsync(entity, ct);
                await Repository.SaveChangesAsync(ct);

                return BaseResponse<object?>.Ok(null, "Deleted.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stock adjustment item delete failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<object?>.Fail("Delete failed.");
        }
    }
}
