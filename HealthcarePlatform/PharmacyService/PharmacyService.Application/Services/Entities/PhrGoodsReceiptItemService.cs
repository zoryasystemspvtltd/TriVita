using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Domain.Entities;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Repositories;
using PharmacyService.Application.Services.Extended;
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
    private readonly IRepository<PhrBatchStock> _batchStocks;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrGoodsReceiptItemService(
        IRepository<PhrGoodsReceiptItem> repository,
        IRepository<PhrGoodsReceipt> goodsReceipts,
        IRepository<PhrPurchaseOrder> purchaseOrders,
        IRepository<PhrPurchaseOrderItem> purchaseOrderItems,
        IRepository<PhrMedicineBatch> batches,
        IRepository<PhrBatchStock> batchStocks,
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
        _batchStocks = batchStocks;
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
                // PO validation (mode rules + remaining qty)
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

                var existingStocks = await _batchStocks.ListAsync(s => s.MedicineBatchId == batch.Id, ct);
                var stock = existingStocks.FirstOrDefault();

                if (stock is null)
                {
                    stock = new PhrBatchStock
                    {
                        MedicineBatchId = batch.Id,
                        CurrentQty = dto.QuantityReceived,
                        ReservedQty = 0m,
                        AvailableQty = dto.QuantityReceived,
                        LastUpdatedOn = DateTime.UtcNow,
                        IsActive = true
                    };
                    AuditHelper.ApplyCreate(stock, Tenant);
                    stock.FacilityId = Tenant.FacilityId;
                    await _batchStocks.AddAsync(stock, ct);
                }
                else
                {
                    stock.CurrentQty += dto.QuantityReceived;
                    stock.AvailableQty += dto.QuantityReceived;
                    stock.LastUpdatedOn = DateTime.UtcNow;
                    AuditHelper.ApplyUpdate(stock, Tenant);
                    await _batchStocks.UpdateAsync(stock, ct);
                }

                batch.AvailableQuantity += dto.QuantityReceived;
                batch.PurchaseRate = dto.UnitPrice;
                if (dto.MRP is not null) batch.MRP = dto.MRP;
                AuditHelper.ApplyUpdate(batch, Tenant);
                await _batches.UpdateAsync(batch, ct);

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

                await RecalcGoodsReceiptTotalsAsync(dto.GoodsReceiptId, ct);

                return BaseResponse<GoodsReceiptItemResponseDto>.Ok(Mapper.Map<GoodsReceiptItemResponseDto>(item), "Created.");
            }, cancellationToken);
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
                // Reverse old quantities from stock/batch
                var oldBatch = await _batches.GetByIdAsync(entity.MedicineBatchId, ct);
                if (oldBatch is not null)
                {
                    var oldStockList = await _batchStocks.ListAsync(s => s.MedicineBatchId == oldBatch.Id, ct);
                    var oldStock = oldStockList.FirstOrDefault();
                    if (oldStock is not null)
                    {
                        oldStock.CurrentQty -= entity.QuantityReceived;
                        oldStock.AvailableQty -= entity.QuantityReceived;
                        oldStock.LastUpdatedOn = DateTime.UtcNow;
                        AuditHelper.ApplyUpdate(oldStock, Tenant);
                        await _batchStocks.UpdateAsync(oldStock, ct);
                    }

                    oldBatch.AvailableQuantity -= entity.QuantityReceived;
                    AuditHelper.ApplyUpdate(oldBatch, Tenant);
                    await _batches.UpdateAsync(oldBatch, ct);
                }

                // Mode validation for PO-based receipts (remaining qty excluding this line)
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

                // Resolve (or create) batch from new input
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

                var stockList = await _batchStocks.ListAsync(s => s.MedicineBatchId == batch.Id, ct);
                var stock = stockList.FirstOrDefault();
                if (stock is null)
                {
                    stock = new PhrBatchStock
                    {
                        MedicineBatchId = batch.Id,
                        CurrentQty = dto.QuantityReceived,
                        ReservedQty = 0m,
                        AvailableQty = dto.QuantityReceived,
                        LastUpdatedOn = DateTime.UtcNow,
                        IsActive = true
                    };
                    AuditHelper.ApplyCreate(stock, Tenant);
                    stock.FacilityId = Tenant.FacilityId;
                    await _batchStocks.AddAsync(stock, ct);
                }
                else
                {
                    stock.CurrentQty += dto.QuantityReceived;
                    stock.AvailableQty += dto.QuantityReceived;
                    stock.LastUpdatedOn = DateTime.UtcNow;
                    AuditHelper.ApplyUpdate(stock, Tenant);
                    await _batchStocks.UpdateAsync(stock, ct);
                }

                batch.AvailableQuantity += dto.QuantityReceived;
                batch.PurchaseRate = dto.UnitPrice;
                if (dto.MRP is not null) batch.MRP = dto.MRP;
                AuditHelper.ApplyUpdate(batch, Tenant);
                await _batches.UpdateAsync(batch, ct);

                // Update entity fields
                Mapper.Map(dto, entity);
                entity.MedicineBatchId = batch.Id;
                entity.BatchNo = batchNo;
                entity.PurchaseRate = dto.UnitPrice;
                entity.LineTotal = dto.QuantityReceived * dto.UnitPrice;
                entity.ExpiryDate = dto.ExpiryDate.Date;
                AuditHelper.ApplyUpdate(entity, Tenant);
                await _items.UpdateAsync(entity, ct);
                await _items.SaveChangesAsync(ct);

                await RecalcGoodsReceiptTotalsAsync(entity.GoodsReceiptId, ct);
                return BaseResponse<GoodsReceiptItemResponseDto>.Ok(Mapper.Map<GoodsReceiptItemResponseDto>(entity), "Updated.");
            }, cancellationToken);
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

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                entity.IsDeleted = true;
                entity.IsActive = false;
                AuditHelper.ApplyUpdate(entity, Tenant);
                await _items.UpdateAsync(entity, ct);

                var batch = await _batches.GetByIdAsync(entity.MedicineBatchId, ct);
                if (batch is not null)
                {
                    var stockList = await _batchStocks.ListAsync(s => s.MedicineBatchId == batch.Id, ct);
                    var stock = stockList.FirstOrDefault();
                    if (stock is not null)
                    {
                        stock.CurrentQty -= entity.QuantityReceived;
                        stock.AvailableQty -= entity.QuantityReceived;
                        stock.LastUpdatedOn = DateTime.UtcNow;
                        AuditHelper.ApplyUpdate(stock, Tenant);
                        await _batchStocks.UpdateAsync(stock, ct);
                    }

                    batch.AvailableQuantity -= entity.QuantityReceived;
                    AuditHelper.ApplyUpdate(batch, Tenant);
                    await _batches.UpdateAsync(batch, ct);
                }

                await _items.SaveChangesAsync(ct);

                await RecalcGoodsReceiptTotalsAsync(entity.GoodsReceiptId, ct);
                return BaseResponse<object?>.Ok(null, "Deleted.");
            }, cancellationToken);
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
        var subTotal = lines.Sum(x => x.LineTotal);

        gr.SubTotal = subTotal;
        var taxable = Math.Max(0m, gr.SubTotal - gr.DiscountAmount);
        gr.GstAmount = Math.Round(taxable * (gr.GstPercent / 100m), 4, MidpointRounding.AwayFromZero);
        gr.TotalAmount = Math.Round(gr.SubTotal - gr.DiscountAmount + gr.GstAmount + gr.OtherTaxAmount, 4, MidpointRounding.AwayFromZero);

        AuditHelper.ApplyUpdate(gr, Tenant);
        await _goodsReceipts.UpdateAsync(gr, ct);
        await _goodsReceipts.SaveChangesAsync(ct);
    }
}
