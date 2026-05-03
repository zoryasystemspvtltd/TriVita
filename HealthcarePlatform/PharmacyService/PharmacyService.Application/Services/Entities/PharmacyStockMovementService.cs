using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using PharmacyService.Application.Abstractions;
using PharmacyService.Application.DTOs.Stock;
using PharmacyService.Application.Services;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;

namespace PharmacyService.Application.Services.Entities;

public interface IPharmacyStockMovementService
{
    Task<BaseResponse<long>> ApplyMovementAsync(
        StockLedgerTransactionType transactionType,
        long referenceId,
        long? referenceLineId,
        long medicineId,
        long medicineBatchId,
        decimal quantityDelta,
        DateTime transactionDate,
        decimal? unitCost,
        string? notes,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<IReadOnlyList<StockFefoAllocation>>> AllocateSaleFefoAsync(
        long medicineId,
        decimal quantityNeeded,
        CancellationToken cancellationToken = default);
}

public sealed class PharmacyStockMovementService : IPharmacyStockMovementService
{
    private readonly IRepository<PhrStockLedger> _ledger;
    private readonly IRepository<PhrBatchStock> _batchStock;
    private readonly IRepository<PhrMedicineBatch> _batches;
    private readonly IPharmacyLockedStockReader _lockedStock;
    private readonly ITenantContext _tenant;

    public PharmacyStockMovementService(
        IRepository<PhrStockLedger> ledger,
        IRepository<PhrBatchStock> batchStock,
        IRepository<PhrMedicineBatch> batches,
        IPharmacyLockedStockReader lockedStock,
        ITenantContext tenant)
    {
        _ledger = ledger;
        _batchStock = batchStock;
        _batches = batches;
        _lockedStock = lockedStock;
        _tenant = tenant;
    }

    public async Task<BaseResponse<long>> ApplyMovementAsync(
        StockLedgerTransactionType transactionType,
        long referenceId,
        long? referenceLineId,
        long medicineId,
        long medicineBatchId,
        decimal quantityDelta,
        DateTime transactionDate,
        decimal? unitCost,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<long>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");
        if (quantityDelta == 0)
            return BaseResponse<long>.Fail("Quantity delta must be non-zero.");

        var batch = await _batches.GetByIdAsync(medicineBatchId, cancellationToken);
        if (batch is null || batch.IsDeleted)
            return BaseResponse<long>.Fail("Medicine batch not found.");
        if (batch.TenantId != _tenant.TenantId)
            return BaseResponse<long>.Fail("Medicine batch not found.");
        if (batch.MedicineId != medicineId)
            return BaseResponse<long>.Fail("Medicine does not match batch.");

        var fid = _tenant.FacilityId.Value;
        PhrBatchStock? stock;
        decimal beforeQty;

        if (quantityDelta < 0)
        {
            var requested = -quantityDelta;
            var medicineTotal = await _lockedStock.SumMedicineFacilityStockLockedAsync(medicineId, fid, cancellationToken);
            if (medicineTotal < requested)
                return BaseResponse<long>.Fail(FormatInsufficientStock(medicineTotal, requested));

            stock = await _lockedStock.GetBatchStockRowLockedAsync(medicineBatchId, fid, cancellationToken);
            beforeQty = stock?.CurrentQty ?? 0m;
            if (beforeQty < requested)
                return BaseResponse<long>.Fail(FormatInsufficientStock(beforeQty, requested));
        }
        else
        {
            var stocks = await _batchStock.ListAsync(
                s => s.MedicineBatchId == medicineBatchId
                    && s.FacilityId == fid
                    && s.TenantId == _tenant.TenantId
                    && !s.IsDeleted,
                cancellationToken);
            stock = stocks.FirstOrDefault();
            beforeQty = stock?.CurrentQty ?? 0;
        }

        var afterQty = beforeQty + quantityDelta;
        if (afterQty < 0)
            return BaseResponse<long>.Fail(FormatInsufficientStock(beforeQty, -quantityDelta));

        decimal? totalCost = unitCost.HasValue
            ? Math.Round(unitCost.Value * Math.Abs(quantityDelta), 4, MidpointRounding.AwayFromZero)
            : null;

        var row = new PhrStockLedger
        {
            TransactionType = transactionType,
            ReferenceId = referenceId,
            ReferenceLineId = referenceLineId,
            MedicineId = medicineId,
            MedicineBatchId = medicineBatchId,
            QuantityDelta = quantityDelta,
            BeforeQty = beforeQty,
            AfterQty = afterQty,
            TransactionDate = transactionDate,
            UnitCost = unitCost,
            TotalCost = totalCost,
            Notes = notes,
            IsActive = true
        };
        AuditHelper.ApplyCreate(row, _tenant);
        row.FacilityId = fid;

        await _ledger.AddAsync(row, cancellationToken);

        if (stock is null)
        {
            if (quantityDelta <= 0)
                return BaseResponse<long>.Fail(FormatInsufficientStock(0m, -quantityDelta));

            stock = new PhrBatchStock
            {
                MedicineBatchId = medicineBatchId,
                CurrentQty = afterQty,
                ReservedQty = 0m,
                AvailableQty = afterQty,
                LastUpdatedOn = DateTime.UtcNow,
                IsActive = true
            };
            AuditHelper.ApplyCreate(stock, _tenant);
            stock.FacilityId = fid;
            await _batchStock.AddAsync(stock, cancellationToken);
        }
        else
        {
            stock.CurrentQty = afterQty;
            stock.AvailableQty = Math.Max(0m, stock.CurrentQty - stock.ReservedQty);
            stock.LastUpdatedOn = DateTime.UtcNow;
            AuditHelper.ApplyUpdate(stock, _tenant);
            await _batchStock.UpdateAsync(stock, cancellationToken);
        }

        batch.AvailableQuantity += quantityDelta;
        AuditHelper.ApplyUpdate(batch, _tenant);
        await _batches.UpdateAsync(batch, cancellationToken);
        return BaseResponse<long>.Ok(row.Id);
    }

    public async Task<BaseResponse<IReadOnlyList<StockFefoAllocation>>> AllocateSaleFefoAsync(
        long medicineId,
        decimal quantityNeeded,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<IReadOnlyList<StockFefoAllocation>>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");
        if (quantityNeeded <= 0)
            return BaseResponse<IReadOnlyList<StockFefoAllocation>>.Fail("Quantity must be greater than zero.");

        var fid = _tenant.FacilityId.Value;
        var medicineTotal = await _lockedStock.SumMedicineFacilityStockLockedAsync(medicineId, fid, cancellationToken);
        if (medicineTotal < quantityNeeded)
            return BaseResponse<IReadOnlyList<StockFefoAllocation>>.Fail(FormatInsufficientStock(medicineTotal, quantityNeeded));

        var batchEntities = await _batches.ListAsync(
            b => b.MedicineId == medicineId && !b.IsDeleted && b.TenantId == _tenant.TenantId,
            cancellationToken);

        var ordered = batchEntities
            .OrderBy(b => b.ExpiryDate.HasValue ? 0 : 1)
            .ThenBy(b => b.ExpiryDate)
            .ThenBy(b => b.Id)
            .ToList();

        var allocations = new List<StockFefoAllocation>();
        var remaining = quantityNeeded;

        foreach (var b in ordered)
        {
            if (remaining <= 0)
                break;

            var st = await _lockedStock.GetBatchStockRowLockedAsync(b.Id, fid, cancellationToken);
            var onHand = st?.CurrentQty ?? 0m;
            if (onHand <= 0)
                continue;

            var take = remaining < onHand ? remaining : onHand;
            allocations.Add(new StockFefoAllocation(b.Id, take));
            remaining -= take;
        }

        if (remaining > 0)
            return BaseResponse<IReadOnlyList<StockFefoAllocation>>.Fail(FormatInsufficientStock(medicineTotal, quantityNeeded));

        return BaseResponse<IReadOnlyList<StockFefoAllocation>>.Ok(allocations);
    }

    private static string FormatInsufficientStock(decimal available, decimal requested) =>
        $"Insufficient stock. Available: {available:G29}, Requested: {requested:G29}.";
}
