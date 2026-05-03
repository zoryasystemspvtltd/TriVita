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

public interface IPhrGoodsReceiptService
{
    Task<BaseResponse<GoodsReceiptResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<GoodsReceiptResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IReadOnlyList<GoodsReceiptPickListDto>>> ListForPurchaseBillAsync(long? purchaseOrderId, CancellationToken cancellationToken = default);
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
    private readonly IPharmacyStockMovementService _stockMovement;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrGoodsReceiptService(
        IRepository<PhrGoodsReceipt> repository,
        IRepository<PhrGoodsReceiptItem> items,
        IRepository<PhrPurchaseOrder> purchaseOrders,
        IRepository<PhrPurchaseOrderItem> purchaseOrderItems,
        IRepository<PhrMedicineBatch> batches,
        IPharmacyStockMovementService stockMovement,
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
        _stockMovement = stockMovement;
        _uow = uow;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<GoodsReceiptResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public async Task<BaseResponse<IReadOnlyList<GoodsReceiptPickListDto>>> ListForPurchaseBillAsync(
        long? purchaseOrderId,
        CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && Tenant.FacilityId is null)
            return BaseResponse<IReadOnlyList<GoodsReceiptPickListDto>>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        var fid = Tenant.FacilityId;
        IReadOnlyList<PhrGoodsReceipt> list;
        if (purchaseOrderId is { } po)
        {
            list = await _goodsReceipts.ListAsync(
                g => !g.IsDeleted && (fid == null || g.FacilityId == fid) && g.PurchaseOrderId == po,
                cancellationToken);
        }
        else
        {
            list = await _goodsReceipts.ListAsync(
                g => !g.IsDeleted && (fid == null || g.FacilityId == fid) && g.PurchaseOrderId == null,
                cancellationToken);
        }

        var dtos = list
            .OrderByDescending(g => g.ReceivedOn)
            .ThenByDescending(g => g.Id)
            .Select(g => new GoodsReceiptPickListDto
            {
                Id = g.Id,
                GoodsReceiptNo = g.GoodsReceiptNo,
                PurchaseOrderId = g.PurchaseOrderId,
                SupplierId = g.SupplierId,
                ReceivedOn = g.ReceivedOn
            })
            .ToList();

        return BaseResponse<IReadOnlyList<GoodsReceiptPickListDto>>.Ok(dtos);
    }

    public override async Task<BaseResponse<GoodsReceiptResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _goodsReceipts.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<GoodsReceiptResponseDto>.Fail("GoodsReceipt not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<GoodsReceiptResponseDto>.Fail("Resource is not in the current facility scope.");

        var dto = Mapper.Map<GoodsReceiptResponseDto>(entity);
        var lines = await _items.ListAsync(x => x.GoodsReceiptId == id && !x.IsDeleted, cancellationToken);
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
                var outLines = await _items.ListAsync(x => x.GoodsReceiptId == entity.Id && !x.IsDeleted, ct);
                outDto.Items = outLines.OrderBy(x => x.LineNum).Select(x => Mapper.Map<GoodsReceiptItemResponseDto>(x)).ToList();
                return BaseResponse<GoodsReceiptResponseDto>.Ok(outDto, "Created.");
            }, cancellationToken, IsolationLevel.Serializable);
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
                var outLines = await _items.ListAsync(x => x.GoodsReceiptId == id && !x.IsDeleted, ct);
                outDto.Items = outLines.OrderBy(x => x.LineNum).Select(x => Mapper.Map<GoodsReceiptItemResponseDto>(x)).ToList();
                return BaseResponse<GoodsReceiptResponseDto>.Ok(outDto, "Updated.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GRN update failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<GoodsReceiptResponseDto>.Fail("Update failed.");
        }
    }

    public override async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _goodsReceipts.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<object?>.Fail("GoodsReceipt not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var lines = await _items.ListAsync(x => x.GoodsReceiptId == id && !x.IsDeleted, ct);
                var grnDate = entity.ReceivedOn == default ? DateTime.UtcNow : entity.ReceivedOn;
                var grnX = new StockLedgerMovementExtras { SourceReference = entity.GoodsReceiptNo, GrnSupplierId = entity.SupplierId };

                foreach (var line in lines)
                {
                    var rev = await _stockMovement.ApplyMovementAsync(
                        StockLedgerTransactionType.GRN,
                        entity.Id,
                        line.Id,
                        line.MedicineId,
                        line.MedicineBatchId,
                        -line.QuantityReceived,
                        grnDate,
                        line.PurchaseRate,
                        "GRN deleted",
                        grnX,
                        ct);
                    if (!rev.Success)
                        return BaseResponse<object?>.Fail(rev.Message ?? "Stock reversal failed.");

                    line.IsDeleted = true;
                    line.IsActive = false;
                    AuditHelper.ApplyUpdate(line, Tenant);
                    await _items.UpdateAsync(line, ct);
                }

                entity.IsDeleted = true;
                entity.IsActive = false;
                AuditHelper.ApplyUpdate(entity, Tenant);
                await _goodsReceipts.UpdateAsync(entity, ct);

                await _items.SaveChangesAsync(ct);
                await _goodsReceipts.SaveChangesAsync(ct);

                return BaseResponse<object?>.Ok(null, "Deleted.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "GRN delete failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<object?>.Fail("Delete failed.");
        }
    }

    private async Task UpsertLinesAsync(PhrGoodsReceipt receipt, List<GoodsReceiptItemUpsertDto> lines, CancellationToken ct)
    {
        var existing = await _items.ListAsync(x => x.GoodsReceiptId == receipt.Id && !x.IsDeleted, ct);
        var byId = existing.ToDictionary(x => x.Id, x => x);
        var keep = new HashSet<long>();
        var grnDate = receipt.ReceivedOn == default ? DateTime.UtcNow : receipt.ReceivedOn;
        var grnX = new StockLedgerMovementExtras { SourceReference = receipt.GoodsReceiptNo, GrnSupplierId = receipt.SupplierId };

        var lineNum = 1;
        foreach (var dto in lines)
        {
            if (dto.MedicineId <= 0) throw new InvalidOperationException("MedicineId is required.");
            if (dto.QuantityReceived <= 0) throw new InvalidOperationException("QuantityReceived must be greater than zero.");
            if (dto.UnitPrice <= 0) throw new InvalidOperationException("UnitPrice must be greater than zero.");
            if (string.IsNullOrWhiteSpace(dto.BatchNo)) throw new InvalidOperationException("BatchNo is required.");
            if (dto.ExpiryDate == default) throw new InvalidOperationException("ExpiryDate is required.");

            if (receipt.PurchaseOrderId is { } poId)
            {
                if (dto.PurchaseOrderItemId is not { } poiId || poiId <= 0)
                    throw new InvalidOperationException("PurchaseOrderItemId is required for GRN with PO.");
                var poi = await _purchaseOrderItems.GetByIdAsync(poiId, ct);
                if (poi is null) throw new InvalidOperationException("Purchase order item not found.");
                if (poi.PurchaseOrderId != poId) throw new InvalidOperationException("PurchaseOrderItemId does not belong to the selected PurchaseOrder.");
                if (poi.MedicineId != dto.MedicineId) throw new InvalidOperationException("MedicineId must match the selected PurchaseOrderItem.");

                var linesForPoi = await _items.ListAsync(
                    x => x.GoodsReceiptId == receipt.Id && x.PurchaseOrderItemId == poiId && !x.IsDeleted,
                    ct);
                decimal receivedOthers = dto.Id is long lid && lid > 0 && byId.TryGetValue(lid, out var curLine)
                    ? linesForPoi.Where(x => x.Id != curLine.Id).Sum(x => x.QuantityReceived)
                    : linesForPoi.Sum(x => x.QuantityReceived);

                if (receivedOthers + dto.QuantityReceived > poi.QuantityOrdered)
                    throw new InvalidOperationException(
                        $"ReceivedQuantity cannot exceed remaining PO quantity for line. Remaining: {poi.QuantityOrdered - receivedOthers}.");
            }
            else
            {
                if (dto.PurchaseOrderItemId is not null)
                    throw new InvalidOperationException("PurchaseOrderItemId must be null for GRN without PO.");
                if (receipt.SupplierId is null || receipt.SupplierId <= 0)
                    throw new InvalidOperationException("SupplierId is required on Goods Receipt when PurchaseOrderId is null.");
            }

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
                var rev = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.GRN,
                    receipt.Id,
                    ent.Id,
                    ent.MedicineId,
                    ent.MedicineBatchId,
                    -ent.QuantityReceived,
                    grnDate,
                    ent.PurchaseRate,
                    "GRN bulk upsert reverse",
                    grnX,
                    ct);
                if (!rev.Success)
                    throw new InvalidOperationException(rev.Message ?? "Stock reversal failed.");

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
                await _items.SaveChangesAsync(ct);

                var mv = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.GRN,
                    receipt.Id,
                    ent.Id,
                    dto.MedicineId,
                    batch.Id,
                    dto.QuantityReceived,
                    grnDate,
                    dto.UnitPrice,
                    null,
                    grnX,
                    ct);
                if (!mv.Success)
                    throw new InvalidOperationException(mv.Message ?? "Stock movement failed.");

                batch.PurchaseRate = dto.UnitPrice;
                if (dto.MRP is not null) batch.MRP = dto.MRP;
                AuditHelper.ApplyUpdate(batch, Tenant);
                await _batches.UpdateAsync(batch, ct);

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
                await _items.SaveChangesAsync(ct);

                var mv = await _stockMovement.ApplyMovementAsync(
                    StockLedgerTransactionType.GRN,
                    receipt.Id,
                    newEnt.Id,
                    dto.MedicineId,
                    batch.Id,
                    dto.QuantityReceived,
                    grnDate,
                    dto.UnitPrice,
                    null,
                    grnX,
                    ct);
                if (!mv.Success)
                    throw new InvalidOperationException(mv.Message ?? "Stock movement failed.");

                batch.PurchaseRate = dto.UnitPrice;
                if (dto.MRP is not null) batch.MRP = dto.MRP;
                AuditHelper.ApplyUpdate(batch, Tenant);
                await _batches.UpdateAsync(batch, ct);

                keep.Add(newEnt.Id);
            }
        }

        foreach (var ent in existing)
        {
            if (keep.Contains(ent.Id)) continue;

            var rev = await _stockMovement.ApplyMovementAsync(
                StockLedgerTransactionType.GRN,
                receipt.Id,
                ent.Id,
                ent.MedicineId,
                ent.MedicineBatchId,
                -ent.QuantityReceived,
                grnDate,
                ent.PurchaseRate,
                "GRN bulk upsert line removed",
                grnX,
                ct);
            if (!rev.Success)
                throw new InvalidOperationException(rev.Message ?? "Stock reversal failed.");

            ent.IsDeleted = true;
            ent.IsActive = false;
            AuditHelper.ApplyUpdate(ent, Tenant);
            await _items.UpdateAsync(ent, ct);
        }

        await _items.SaveChangesAsync(ct);
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

        var lines = await _items.ListAsync(x => x.GoodsReceiptId == goodsReceiptId && !x.IsDeleted, ct);
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
