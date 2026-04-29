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

public interface IPhrGoodsReceiptService
{
    Task<BaseResponse<GoodsReceiptResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<GoodsReceiptResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<GoodsReceiptResponseDto>> CreateAsync(CreateGoodsReceiptDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<GoodsReceiptResponseDto>> UpdateAsync(long id, UpdateGoodsReceiptDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrGoodsReceiptService : PhrCrudServiceBase<PhrGoodsReceipt, CreateGoodsReceiptDto, UpdateGoodsReceiptDto, GoodsReceiptResponseDto, PhrGoodsReceiptService>, IPhrGoodsReceiptService
{
    private readonly IRepository<PhrGoodsReceipt> _goodsReceipts;
    private readonly IRepository<PhrGoodsReceiptItem> _items;
    private readonly IRepository<PhrPurchaseOrder> _purchaseOrders;
    private readonly IRepository<PhrPurchaseOrderItem> _purchaseOrderItems;
    private readonly IRepository<PhrMedicineBatch> _batches;
    private readonly IRepository<PhrBatchStock> _batchStocks;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrGoodsReceiptService(
        IRepository<PhrGoodsReceipt> repository,
        IRepository<PhrGoodsReceiptItem> items,
        IRepository<PhrPurchaseOrder> purchaseOrders,
        IRepository<PhrPurchaseOrderItem> purchaseOrderItems,
        IRepository<PhrMedicineBatch> batches,
        IRepository<PhrBatchStock> batchStocks,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateGoodsReceiptDto>? createValidator,
        IValidator<UpdateGoodsReceiptDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrGoodsReceiptService> logger,
        IPharmacyUnitOfWork uow)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _goodsReceipts = repository;
        _items = items;
        _purchaseOrders = purchaseOrders;
        _purchaseOrderItems = purchaseOrderItems;
        _batches = batches;
        _batchStocks = batchStocks;
        _uow = uow;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<GoodsReceiptResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<GoodsReceiptResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _goodsReceipts.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<GoodsReceiptResponseDto>.Fail("GoodsReceipt not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<GoodsReceiptResponseDto>.Fail("Resource is not in the current facility scope.");

        var dto = Mapper.Map<GoodsReceiptResponseDto>(entity);
        var lines = await _items.ListAsync(x => x.GoodsReceiptId == id, cancellationToken);
        dto.Items = lines.OrderBy(x => x.LineNum).Select(x => Mapper.Map<GoodsReceiptItemResponseDto>(x)).ToList();
        return BaseResponse<GoodsReceiptResponseDto>.Ok(dto);
    }

    public override async Task<BaseResponse<GoodsReceiptResponseDto>> CreateAsync(
        CreateGoodsReceiptDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return BaseResponse<GoodsReceiptResponseDto>.Fail("At least one item is required.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var entity = Mapper.Map<PhrGoodsReceipt>(dto);
                AuditHelper.ApplyCreate(entity, Tenant);
                entity.FacilityId = Tenant.FacilityId;

                // If with PO, inherit billing terms
                if (entity.PurchaseOrderId is { } poId)
                {
                    var po = await _purchaseOrders.GetByIdAsync(poId, ct);
                    if (po is not null)
                    {
                        entity.DiscountAmount = po.DiscountAmount;
                        entity.GstPercent = po.GstPercent;
                        entity.OtherTaxAmount = po.OtherTaxAmount;
                    }
                }

                await _goodsReceipts.AddAsync(entity, ct);
                await _goodsReceipts.SaveChangesAsync(ct);

                await UpsertLinesAsync(entity, dto.Items, ct);
                await RecalcTotalsAsync(entity.Id, ct);

                var fresh = await _goodsReceipts.GetByIdAsync(entity.Id, ct);
                if (fresh is null) return BaseResponse<GoodsReceiptResponseDto>.Fail("GoodsReceipt not found.");
                var outDto = Mapper.Map<GoodsReceiptResponseDto>(fresh);
                var outLines = await _items.ListAsync(x => x.GoodsReceiptId == entity.Id, ct);
                outDto.Items = outLines.OrderBy(x => x.LineNum).Select(x => Mapper.Map<GoodsReceiptItemResponseDto>(x)).ToList();
                return BaseResponse<GoodsReceiptResponseDto>.Ok(outDto, "Created.");
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GRN create failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<GoodsReceiptResponseDto>.Fail("Create failed.");
        }
    }

    public override async Task<BaseResponse<GoodsReceiptResponseDto>> UpdateAsync(
        long id,
        UpdateGoodsReceiptDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return BaseResponse<GoodsReceiptResponseDto>.Fail("At least one item is required.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var entity = await _goodsReceipts.GetByIdAsync(id, ct);
                if (entity is null) return BaseResponse<GoodsReceiptResponseDto>.Fail("GoodsReceipt not found.");
                if (!IsEntityInFacilityScope(entity)) return BaseResponse<GoodsReceiptResponseDto>.Fail("Resource is not in the current facility scope.");

                entity.GoodsReceiptNo = dto.GoodsReceiptNo;
                entity.PurchaseOrderId = dto.PurchaseOrderId;
                entity.SupplierId = dto.SupplierId;
                entity.ReceivedOn = dto.ReceivedOn;
                entity.ReceivedByDoctorId = dto.ReceivedByDoctorId;
                entity.StatusReferenceValueId = dto.StatusReferenceValueId;
                entity.Notes = dto.Notes;

                if (entity.PurchaseOrderId is { } poId)
                {
                    var po = await _purchaseOrders.GetByIdAsync(poId, ct);
                    if (po is not null)
                    {
                        entity.DiscountAmount = po.DiscountAmount;
                        entity.GstPercent = po.GstPercent;
                        entity.OtherTaxAmount = po.OtherTaxAmount;
                    }
                }
                else
                {
                    entity.DiscountAmount = dto.DiscountAmount;
                    entity.GstPercent = dto.GstPercent;
                    entity.OtherTaxAmount = dto.OtherTaxAmount;
                }

                AuditHelper.ApplyUpdate(entity, Tenant);
                await _goodsReceipts.UpdateAsync(entity, ct);
                await _goodsReceipts.SaveChangesAsync(ct);

                await UpsertLinesAsync(entity, dto.Items, ct);
                await RecalcTotalsAsync(id, ct);

                var fresh = await _goodsReceipts.GetByIdAsync(id, ct);
                if (fresh is null) return BaseResponse<GoodsReceiptResponseDto>.Fail("GoodsReceipt not found.");
                var outDto = Mapper.Map<GoodsReceiptResponseDto>(fresh);
                var outLines = await _items.ListAsync(x => x.GoodsReceiptId == id, ct);
                outDto.Items = outLines.OrderBy(x => x.LineNum).Select(x => Mapper.Map<GoodsReceiptItemResponseDto>(x)).ToList();
                return BaseResponse<GoodsReceiptResponseDto>.Ok(outDto, "Updated.");
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GRN update failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<GoodsReceiptResponseDto>.Fail("Update failed.");
        }
    }

    private async Task UpsertLinesAsync(PhrGoodsReceipt receipt, List<GoodsReceiptItemUpsertDto> lines, CancellationToken ct)
    {
        var existing = await _items.ListAsync(x => x.GoodsReceiptId == receipt.Id, ct);
        var byId = existing.ToDictionary(x => x.Id, x => x);
        var keep = new HashSet<long>();

        var lineNum = 1;
        foreach (var dto in lines)
        {
            if (dto.MedicineId <= 0) throw new InvalidOperationException("MedicineId is required.");
            if (dto.QuantityReceived <= 0) throw new InvalidOperationException("QuantityReceived must be greater than zero.");
            if (dto.UnitPrice <= 0) throw new InvalidOperationException("UnitPrice must be greater than zero.");
            if (string.IsNullOrWhiteSpace(dto.BatchNo)) throw new InvalidOperationException("BatchNo is required.");
            if (dto.ExpiryDate == default) throw new InvalidOperationException("ExpiryDate is required.");

            // Validate PO mode fields
            if (receipt.PurchaseOrderId is { } poId)
            {
                if (dto.PurchaseOrderItemId is not { } poiId || poiId <= 0)
                    throw new InvalidOperationException("PurchaseOrderItemId is required for GRN with PO.");
                var poi = await _purchaseOrderItems.GetByIdAsync(poiId, ct);
                if (poi is null) throw new InvalidOperationException("Purchase order item not found.");
                if (poi.PurchaseOrderId != poId) throw new InvalidOperationException("PurchaseOrderItemId does not belong to the selected PurchaseOrder.");
                if (poi.MedicineId != dto.MedicineId) throw new InvalidOperationException("MedicineId must match the selected PurchaseOrderItem.");
            }
            else
            {
                if (dto.PurchaseOrderItemId is not null)
                    throw new InvalidOperationException("PurchaseOrderItemId must be null for GRN without PO.");
                if (receipt.SupplierId is null || receipt.SupplierId <= 0)
                    throw new InvalidOperationException("SupplierId is required on Goods Receipt when PurchaseOrderId is null.");
            }

            // Resolve/create batch
            var batchNo = dto.BatchNo.Trim();
            var batchList = await _batches.ListAsync(b => b.MedicineId == dto.MedicineId && b.BatchNo == batchNo, ct);
            var batch = batchList.FirstOrDefault();
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
                    CreatedFromGoodsReceiptId = receipt.Id,
                    IsActive = true
                };
                AuditHelper.ApplyCreate(batch, Tenant);
                await _batches.AddAsync(batch, ct);
                await _batches.SaveChangesAsync(ct);
            }
            else if (batch.ExpiryDate is { } ex && ex.Date != dto.ExpiryDate.Date)
            {
                throw new InvalidOperationException("Batch already exists with a different ExpiryDate for the same medicine.");
            }

            if (dto.Id is { } id && id > 0 && byId.TryGetValue(id, out var ent))
            {
                // reverse old qty from stock/batch
                var oldBatch = await _batches.GetByIdAsync(ent.MedicineBatchId, ct);
                if (oldBatch is not null)
                {
                    var oldStockList = await _batchStocks.ListAsync(s => s.MedicineBatchId == oldBatch.Id, ct);
                    var oldStock = oldStockList.FirstOrDefault();
                    if (oldStock is not null)
                    {
                        oldStock.CurrentQty -= ent.QuantityReceived;
                        oldStock.AvailableQty -= ent.QuantityReceived;
                        oldStock.LastUpdatedOn = DateTime.UtcNow;
                        AuditHelper.ApplyUpdate(oldStock, Tenant);
                        await _batchStocks.UpdateAsync(oldStock, ct);
                    }
                    oldBatch.AvailableQuantity -= ent.QuantityReceived;
                    AuditHelper.ApplyUpdate(oldBatch, Tenant);
                    await _batches.UpdateAsync(oldBatch, ct);
                }

                ent.LineNum = lineNum++;
                ent.PurchaseOrderItemId = dto.PurchaseOrderItemId;
                ent.MedicineId = dto.MedicineId;
                ent.MedicineBatchId = batch.Id;
                ent.BatchNo = batchNo;
                ent.QuantityReceived = dto.QuantityReceived;
                ent.PurchaseRate = dto.UnitPrice;
                ent.LineTotal = dto.QuantityReceived * dto.UnitPrice;
                ent.ExpiryDate = dto.ExpiryDate.Date;
                ent.MRP = dto.MRP;
                AuditHelper.ApplyUpdate(ent, Tenant);
                await _items.UpdateAsync(ent, ct);
                keep.Add(ent.Id);
            }
            else
            {
                var newEnt = new PhrGoodsReceiptItem
                {
                    GoodsReceiptId = receipt.Id,
                    PurchaseOrderItemId = dto.PurchaseOrderItemId,
                    LineNum = lineNum++,
                    MedicineId = dto.MedicineId,
                    MedicineBatchId = batch.Id,
                    BatchNo = batchNo,
                    QuantityReceived = dto.QuantityReceived,
                    PurchaseRate = dto.UnitPrice,
                    LineTotal = dto.QuantityReceived * dto.UnitPrice,
                    ExpiryDate = dto.ExpiryDate.Date,
                    MRP = dto.MRP,
                    IsActive = true
                };
                AuditHelper.ApplyCreate(newEnt, Tenant);
                newEnt.FacilityId = Tenant.FacilityId;
                await _items.AddAsync(newEnt, ct);
            }

            // apply new qty to stock/batch
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
        }

        foreach (var ent in existing)
        {
            if (keep.Contains(ent.Id)) continue;
            ent.IsDeleted = true;
            ent.IsActive = false;
            AuditHelper.ApplyUpdate(ent, Tenant);
            await _items.UpdateAsync(ent, ct);
        }

        await _items.SaveChangesAsync(ct);
        await _batchStocks.SaveChangesAsync(ct);
        await _batches.SaveChangesAsync(ct);
    }

    private async Task RecalcTotalsAsync(long goodsReceiptId, CancellationToken ct)
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
