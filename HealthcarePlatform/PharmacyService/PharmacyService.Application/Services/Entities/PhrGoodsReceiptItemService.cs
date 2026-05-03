using System.Data;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.DTOs.Stock;
using PharmacyService.Application.Services;
using PharmacyService.Application.Services.Extended;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrGoodsReceiptItemService
{
    Task<BaseResponse<GoodsReceiptItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<GoodsReceiptItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<GoodsReceiptItemResponseDto>> CreateAsync(CreateGoodsReceiptItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<GoodsReceiptItemResponseDto>> UpdateAsync(long id, UpdateGoodsReceiptItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrGoodsReceiptItemService : PhrCrudServiceBase<PhrGoodsReceiptItem, CreateGoodsReceiptItemDto, UpdateGoodsReceiptItemDto, GoodsReceiptItemResponseDto, PhrGoodsReceiptItemService>, IPhrGoodsReceiptItemService
{
    private readonly IRepository<PhrGoodsReceipt> _goodsReceipts;
    private readonly IRepository<PhrGoodsReceiptItem> _items;
    private readonly IRepository<PhrPurchaseOrderItem> _purchaseOrderItems;
    private readonly IRepository<PhrPurchaseOrder> _purchaseOrders;
    private readonly IRepository<PhrMedicineBatch> _batches;
    private readonly IPharmacyStockMovementService _stockMovement;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrGoodsReceiptItemService(
        IRepository<PhrGoodsReceiptItem> repository,
        IRepository<PhrGoodsReceipt> goodsReceipts,
        IRepository<PhrPurchaseOrder> purchaseOrders,
        IRepository<PhrPurchaseOrderItem> purchaseOrderItems,
        IRepository<PhrMedicineBatch> batches,
        IPharmacyStockMovementService stockMovement,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateGoodsReceiptItemDto>? createValidator,
        IValidator<UpdateGoodsReceiptItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrGoodsReceiptItemService> logger,
        IPharmacyUnitOfWork uow)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _goodsReceipts = goodsReceipts;
        _items = repository;
        _purchaseOrders = purchaseOrders;
        _purchaseOrderItems = purchaseOrderItems;
        _batches = batches;
        _stockMovement = stockMovement;
        _uow = uow;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<GoodsReceiptItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<GoodsReceiptItemResponseDto>> CreateAsync(
        CreateGoodsReceiptItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantityReceived <= 0) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("ReceivedQuantity must be greater than zero.");
        if (dto.UnitPrice <= 0) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("UnitPrice must be greater than zero.");
        if (string.IsNullOrWhiteSpace(dto.BatchNo)) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("BatchNo is required.");
        if (dto.ExpiryDate == default) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("ExpiryDate is required.");

        var receipt = await _goodsReceipts.GetByIdAsync(dto.GoodsReceiptId, cancellationToken);
        if (receipt is null) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Goods receipt not found.");
        if (Tenant.FacilityId is long fid && receipt.FacilityId is long rfid && rfid != fid)
            return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Resource is not in the current facility scope.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                if (receipt.PurchaseOrderId is { } poId)
                {
                    if (dto.PurchaseOrderItemId is not { } poiId || poiId <= 0)
                        return BaseResponse<GoodsReceiptItemResponseDto>.Fail("PurchaseOrderItemId is required for GRN with PO.");

                    var poi = await _purchaseOrderItems.GetByIdAsync(poiId, ct);
                    if (poi is null) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Purchase order item not found.");
                    if (poi.PurchaseOrderId != poId) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("PurchaseOrderItemId does not belong to the selected PurchaseOrder.");
                    if (poi.MedicineId != dto.MedicineId) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("MedicineId must match the selected PurchaseOrderItem.");

                    var existingForPoItem = await _items.ListAsync(x => x.PurchaseOrderItemId == poiId, ct);
                    var alreadyReceived = existingForPoItem.Sum(x => x.QuantityReceived);
                    var remaining = poi.QuantityOrdered - alreadyReceived;
                    if (dto.QuantityReceived > remaining)
                        return BaseResponse<GoodsReceiptItemResponseDto>.Fail($"ReceivedQuantity cannot exceed remaining PO quantity. Remaining: {remaining}.");
                }
                else
                {
                    if (dto.PurchaseOrderItemId is not null) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("PurchaseOrderItemId must be null for GRN without PO.");
                    if (receipt.SupplierId is null || receipt.SupplierId <= 0)
                        return BaseResponse<GoodsReceiptItemResponseDto>.Fail("SupplierId is required on Goods Receipt when PurchaseOrderId is null.");
                }

                var batchNo = dto.BatchNo.Trim();
                var existingBatches = await _batches.ListAsync(b => b.MedicineId == dto.MedicineId && b.BatchNo == batchNo, ct);
                var batch = existingBatches.FirstOrDefault();

                if (batch is null)
                {
                    batch = new PhrMedicineBatch
                    {
                        MedicineId = dto.MedicineId,
                        BatchNo = batchNo,
                        ExpiryDate = dto.ExpiryDate.Date,
                        PurchaseRate = dto.UnitPrice,
                        MRP = dto.MRP,
                        AvailableQuantity = 0m,
                        CreatedFromGoodsReceiptId = dto.GoodsReceiptId,
                        IsActive = true
                    };
                    AuditHelper.ApplyCreate(batch, Tenant);
                    await _batches.AddAsync(batch, ct);
                    await _batches.SaveChangesAsync(ct);
                }
                else
                {
                    if (batch.ExpiryDate is { } ex && ex.Date != dto.ExpiryDate.Date)
                        return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Batch already exists with a different ExpiryDate for the same medicine.");
                }

                var item = Mapper.Map<PhrGoodsReceiptItem>(dto);
                item.MedicineBatchId = batch.Id;
                item.BatchNo = batchNo;
                item.PurchaseRate = dto.UnitPrice;
                item.LineTotal = dto.QuantityReceived * dto.UnitPrice;
                item.ExpiryDate = dto.ExpiryDate.Date;
                AuditHelper.ApplyCreate(item, Tenant);
                item.FacilityId = Tenant.FacilityId;

                await _items.AddAsync(item, ct);
                await _items.SaveChangesAsync(ct);

                var grnDate = receipt.ReceivedOn == default ? DateTime.UtcNow : receipt.ReceivedOn;
                var grnX = new StockLedgerMovementExtras
                {
                    SourceReference = receipt.GoodsReceiptNo,
                    GrnSupplierId = receipt.SupplierId
                };
                var mv = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.GRN,
                    dto.GoodsReceiptId,
                    item.Id,
                    dto.MedicineId,
                    batch.Id,
                    dto.QuantityReceived,
                    grnDate,
                    dto.UnitPrice,
                    null,
                    grnX,
                    ct);
                if (!mv.Success)
                    return BaseResponse<GoodsReceiptItemResponseDto>.Fail(mv.Message ?? "Stock movement failed.");

                batch.PurchaseRate = dto.UnitPrice;
                if (dto.MRP is not null) batch.MRP = dto.MRP;
                AuditHelper.ApplyUpdate(batch, Tenant);
                await _batches.UpdateAsync(batch, ct);

                await RecalcGoodsReceiptTotalsAsync(dto.GoodsReceiptId, ct);

                return BaseResponse<GoodsReceiptItemResponseDto>.Ok(Mapper.Map<GoodsReceiptItemResponseDto>(item), "Created.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GRN item create failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Create failed.");
        }
    }

    public override async Task<BaseResponse<GoodsReceiptItemResponseDto>> UpdateAsync(
        long id,
        UpdateGoodsReceiptItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantityReceived <= 0) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("ReceivedQuantity must be greater than zero.");
        if (dto.UnitPrice <= 0) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("UnitPrice must be greater than zero.");
        if (string.IsNullOrWhiteSpace(dto.BatchNo)) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("BatchNo is required.");
        if (dto.ExpiryDate == default) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("ExpiryDate is required.");

        var entity = await _items.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("GoodsReceiptItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Resource is not in the current facility scope.");

        var receipt = await _goodsReceipts.GetByIdAsync(entity.GoodsReceiptId, cancellationToken);
        if (receipt is null) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Goods receipt not found.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var grnDate = receipt.ReceivedOn == default ? DateTime.UtcNow : receipt.ReceivedOn;

                var grnX = new StockLedgerMovementExtras { SourceReference = receipt.GoodsReceiptNo, GrnSupplierId = receipt.SupplierId };
                var rev = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.GRN,
                    entity.GoodsReceiptId,
                    entity.Id,
                    entity.MedicineId,
                    entity.MedicineBatchId,
                    -entity.QuantityReceived,
                    grnDate,
                    entity.PurchaseRate,
                    "GRN line correction (reverse)",
                    grnX,
                    ct);
                if (!rev.Success)
                    return BaseResponse<GoodsReceiptItemResponseDto>.Fail(rev.Message ?? "Stock reversal failed.");

                if (receipt.PurchaseOrderId is { } poId)
                {
                    if (dto.PurchaseOrderItemId is not { } poiId || poiId <= 0)
                        return BaseResponse<GoodsReceiptItemResponseDto>.Fail("PurchaseOrderItemId is required for GRN with PO.");

                    var poi = await _purchaseOrderItems.GetByIdAsync(poiId, ct);
                    if (poi is null) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Purchase order item not found.");
                    if (poi.PurchaseOrderId != poId) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("PurchaseOrderItemId does not belong to the selected PurchaseOrder.");
                    if (poi.MedicineId != dto.MedicineId) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("MedicineId must match the selected PurchaseOrderItem.");

                    var otherLines = await _items.ListAsync(x => x.PurchaseOrderItemId == poiId && x.Id != id, ct);
                    var alreadyReceived = otherLines.Sum(x => x.QuantityReceived);
                    var remaining = poi.QuantityOrdered - alreadyReceived;
                    if (dto.QuantityReceived > remaining)
                        return BaseResponse<GoodsReceiptItemResponseDto>.Fail($"ReceivedQuantity cannot exceed remaining PO quantity. Remaining: {remaining}.");
                }
                else
                {
                    if (dto.PurchaseOrderItemId is not null) return BaseResponse<GoodsReceiptItemResponseDto>.Fail("PurchaseOrderItemId must be null for GRN without PO.");
                    if (receipt.SupplierId is null || receipt.SupplierId <= 0)
                        return BaseResponse<GoodsReceiptItemResponseDto>.Fail("SupplierId is required on Goods Receipt when PurchaseOrderId is null.");
                }

                var batchNo = dto.BatchNo.Trim();
                var existingBatches = await _batches.ListAsync(b => b.MedicineId == dto.MedicineId && b.BatchNo == batchNo, ct);
                var batch = existingBatches.FirstOrDefault();

                if (batch is null)
                {
                    batch = new PhrMedicineBatch
                    {
                        MedicineId = dto.MedicineId,
                        BatchNo = batchNo,
                        ExpiryDate = dto.ExpiryDate.Date,
                        PurchaseRate = dto.UnitPrice,
                        MRP = dto.MRP,
                        AvailableQuantity = 0m,
                        CreatedFromGoodsReceiptId = entity.GoodsReceiptId,
                        IsActive = true
                    };
                    AuditHelper.ApplyCreate(batch, Tenant);
                    await _batches.AddAsync(batch, ct);
                    await _batches.SaveChangesAsync(ct);
                }
                else
                {
                    if (batch.ExpiryDate is { } ex && ex.Date != dto.ExpiryDate.Date)
                        return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Batch already exists with a different ExpiryDate for the same medicine.");
                }

                Mapper.Map(dto, entity);
                entity.MedicineBatchId = batch.Id;
                entity.BatchNo = batchNo;
                entity.PurchaseRate = dto.UnitPrice;
                entity.LineTotal = dto.QuantityReceived * dto.UnitPrice;
                entity.ExpiryDate = dto.ExpiryDate.Date;
                AuditHelper.ApplyUpdate(entity, Tenant);
                await _items.UpdateAsync(entity, ct);
                await _items.SaveChangesAsync(ct);

                var mv = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.GRN,
                    entity.GoodsReceiptId,
                    entity.Id,
                    dto.MedicineId,
                    batch.Id,
                    dto.QuantityReceived,
                    grnDate,
                    dto.UnitPrice,
                    null,
                    grnX,
                    ct);
                if (!mv.Success)
                    return BaseResponse<GoodsReceiptItemResponseDto>.Fail(mv.Message ?? "Stock movement failed.");

                batch.PurchaseRate = dto.UnitPrice;
                if (dto.MRP is not null) batch.MRP = dto.MRP;
                AuditHelper.ApplyUpdate(batch, Tenant);
                await _batches.UpdateAsync(batch, ct);

                await RecalcGoodsReceiptTotalsAsync(entity.GoodsReceiptId, ct);
                return BaseResponse<GoodsReceiptItemResponseDto>.Ok(Mapper.Map<GoodsReceiptItemResponseDto>(entity), "Updated.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GRN item update failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<GoodsReceiptItemResponseDto>.Fail("Update failed.");
        }
    }

    public override async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _items.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<object?>.Fail("GoodsReceiptItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        var receipt = await _goodsReceipts.GetByIdAsync(entity.GoodsReceiptId, cancellationToken);
        if (receipt is null) return BaseResponse<object?>.Fail("Goods receipt not found.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var grnDate = receipt.ReceivedOn == default ? DateTime.UtcNow : receipt.ReceivedOn;
                var grnX = new StockLedgerMovementExtras { SourceReference = receipt.GoodsReceiptNo, GrnSupplierId = receipt.SupplierId };
                var rev = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.GRN,
                    entity.GoodsReceiptId,
                    entity.Id,
                    entity.MedicineId,
                    entity.MedicineBatchId,
                    -entity.QuantityReceived,
                    grnDate,
                    entity.PurchaseRate,
                    "GRN line deleted",
                    grnX,
                    ct);
                if (!rev.Success)
                    return BaseResponse<object?>.Fail(rev.Message ?? "Stock reversal failed.");

                entity.IsDeleted = true;
                entity.IsActive = false;
                AuditHelper.ApplyUpdate(entity, Tenant);
                await _items.UpdateAsync(entity, ct);

                await _items.SaveChangesAsync(ct);

                await RecalcGoodsReceiptTotalsAsync(entity.GoodsReceiptId, ct);
                return BaseResponse<object?>.Ok(null, "Deleted.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GRN item delete failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<object?>.Fail("Delete failed.");
        }
    }

    private async Task RecalcGoodsReceiptTotalsAsync(long goodsReceiptId, CancellationToken ct)
    {
        var gr = await _goodsReceipts.GetByIdAsync(goodsReceiptId, ct);
        if (gr is null) return;

        if (gr.PurchaseOrderId is { } poId)
        {
            var po = await _purchaseOrders.GetByIdAsync(poId, ct);
            if (po is not null)
            {
                gr.DiscountAmount = po.DiscountAmount;
                gr.GstPercent = po.GstPercent;
                gr.OtherTaxAmount = po.OtherTaxAmount;
            }
        }

        var lines = await _items.ListAsync(x => x.GoodsReceiptId == goodsReceiptId, ct);
        var subTotal = lines.Where(x => !x.IsDeleted).Sum(x => x.LineTotal);

        gr.SubTotal = subTotal;
        var taxable = Math.Max(0m, gr.SubTotal - gr.DiscountAmount);
        gr.GstAmount = Math.Round(taxable * (gr.GstPercent / 100m), 4, MidpointRounding.AwayFromZero);
        gr.TotalAmount = Math.Round(gr.SubTotal - gr.DiscountAmount + gr.GstAmount + gr.OtherTaxAmount, 4, MidpointRounding.AwayFromZero);

        AuditHelper.ApplyUpdate(gr, Tenant);
        await _goodsReceipts.UpdateAsync(gr, ct);
        await _goodsReceipts.SaveChangesAsync(ct);
    }
}
