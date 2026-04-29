using FluentValidation;
using Healthcare.Common.MultiTenancy;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;

namespace PharmacyService.Application.Validation;

public sealed class CreateGoodsReceiptItemDtoValidator : AbstractValidator<CreateGoodsReceiptItemDto>
{
    private readonly IRepository<PhrGoodsReceipt> _goodsReceipts;
    private readonly IRepository<PhrPurchaseOrderItem> _purchaseOrderItems;
    private readonly IRepository<PhrGoodsReceiptItem> _goodsReceiptItems;
    private readonly ITenantContext _tenant;

    public CreateGoodsReceiptItemDtoValidator(
        IRepository<PhrGoodsReceipt> goodsReceipts,
        IRepository<PhrPurchaseOrderItem> purchaseOrderItems,
        IRepository<PhrGoodsReceiptItem> goodsReceiptItems,
        ITenantContext tenant)
    {
        _goodsReceipts = goodsReceipts;
        _purchaseOrderItems = purchaseOrderItems;
        _goodsReceiptItems = goodsReceiptItems;
        _tenant = tenant;

        RuleFor(x => x)
            .CustomAsync(ValidateModeAndQuantitiesAsync);
    }

    private async Task ValidateModeAndQuantitiesAsync(
        CreateGoodsReceiptItemDto dto,
        ValidationContext<CreateGoodsReceiptItemDto> ctx,
        CancellationToken ct)
    {
        if (dto.GoodsReceiptId <= 0)
        {
            ctx.AddFailure("GoodsReceiptId is required.");
            return;
        }

        if (dto.QuantityReceived <= 0)
        {
            ctx.AddFailure("QuantityReceived must be greater than zero.");
            return;
        }

        if (dto.UnitPrice <= 0)
        {
            ctx.AddFailure("UnitPrice must be greater than zero.");
            return;
        }

        if (string.IsNullOrWhiteSpace(dto.BatchNo))
        {
            ctx.AddFailure("BatchNo is required.");
            return;
        }

        if (dto.ExpiryDate == default)
        {
            ctx.AddFailure("ExpiryDate is required.");
            return;
        }

        var receipt = await _goodsReceipts.GetByIdAsync(dto.GoodsReceiptId, ct);
        if (receipt is null)
        {
            ctx.AddFailure("Goods receipt not found.");
            return;
        }

        var facilityId = _tenant.FacilityId;

        // MODE 1: GRN WITH PO
        if (receipt.PurchaseOrderId is { } poId)
        {
            if (dto.PurchaseOrderItemId is not { } poiId)
            {
                ctx.AddFailure("PurchaseOrderItemId is required for GRN with PO.");
                return;
            }

            var poi = await _purchaseOrderItems.GetByIdAsync(poiId, ct);
            if (poi is null)
            {
                ctx.AddFailure("Purchase order item not found.");
                return;
            }

            if (poi.PurchaseOrderId != poId)
            {
                ctx.AddFailure("PurchaseOrderItemId does not belong to the selected PurchaseOrder.");
                return;
            }

            if (poi.MedicineId != dto.MedicineId)
            {
                ctx.AddFailure("MedicineId must match the selected PurchaseOrderItem.");
                return;
            }

            // Partial receipt allowed but not over-receipt.
            IReadOnlyList<PhrGoodsReceiptItem> existingForPoItem = facilityId is long fid2
                ? await _goodsReceiptItems.ListAsync(x =>
                    x.PurchaseOrderItemId == poiId &&
                    x.FacilityId == fid2, ct)
                : await _goodsReceiptItems.ListAsync(x =>
                    x.PurchaseOrderItemId == poiId, ct);

            var alreadyReceived = existingForPoItem.Sum(x => x.QuantityReceived);
            var remaining = poi.QuantityOrdered - alreadyReceived;

            if (remaining < 0)
                ctx.AddFailure("Remaining quantity for this PO item is invalid.");

            if (dto.QuantityReceived > remaining)
            {
                ctx.AddFailure($"ReceivedQuantity cannot exceed remaining PO quantity. Remaining: {remaining}.");
                return;
            }

            return;
        }

        // MODE 2: GRN WITHOUT PO (direct entry)
        if (receipt.PurchaseOrderId is null)
        {
            if (dto.PurchaseOrderItemId is not null)
            {
                ctx.AddFailure("PurchaseOrderItemId must be null for GRN without PO.");
                return;
            }

            if (receipt.SupplierId is null || receipt.SupplierId <= 0)
            {
                ctx.AddFailure("SupplierId is required on Goods Receipt when PurchaseOrderId is null.");
                return;
            }

            // No duplicate item entry in the same GRN for the same medicine+batchno.
            var bn = dto.BatchNo.Trim();
            IReadOnlyList<PhrGoodsReceiptItem> dupeItems = facilityId is long fid3
                ? await _goodsReceiptItems.ListAsync(x =>
                    x.GoodsReceiptId == dto.GoodsReceiptId &&
                    x.PurchaseOrderItemId == null &&
                    x.MedicineId == dto.MedicineId &&
                    x.ExpiryDate == dto.ExpiryDate.Date &&
                    x.FacilityId == fid3, ct)
                : await _goodsReceiptItems.ListAsync(x =>
                    x.GoodsReceiptId == dto.GoodsReceiptId &&
                    x.PurchaseOrderItemId == null &&
                    x.MedicineId == dto.MedicineId &&
                    x.ExpiryDate == dto.ExpiryDate.Date, ct);

            if (dupeItems.Count > 0)
            {
                ctx.AddFailure("Duplicate batch entry is not allowed in the same Goods Receipt.");
                return;
            }
        }
    }
}

