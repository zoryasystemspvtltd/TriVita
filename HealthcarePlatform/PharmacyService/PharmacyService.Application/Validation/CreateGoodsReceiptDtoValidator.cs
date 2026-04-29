using FluentValidation;
using Healthcare.Common.MultiTenancy;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;

namespace PharmacyService.Application.Validation;

public sealed class CreateGoodsReceiptDtoValidator : AbstractValidator<CreateGoodsReceiptDto>
{
    private readonly IRepository<PhrPurchaseOrder> _purchaseOrders;
    private readonly IRepository<PhrSupplier> _suppliers;
    private readonly ITenantContext _tenant;

    public CreateGoodsReceiptDtoValidator(
        IRepository<PhrPurchaseOrder> purchaseOrders,
        IRepository<PhrSupplier> suppliers,
        ITenantContext tenant)
    {
        _purchaseOrders = purchaseOrders;
        _suppliers = suppliers;
        _tenant = tenant;

        RuleFor(x => x.GoodsReceiptNo)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(x => x)
            .CustomAsync(ValidateModeAsync);
    }

    private async Task ValidateModeAsync(CreateGoodsReceiptDto dto, ValidationContext<CreateGoodsReceiptDto> ctx, CancellationToken ct)
    {
        if (dto.PurchaseOrderId is { } poId)
        {
            if (poId <= 0)
            {
                ctx.AddFailure("PurchaseOrderId must be a positive number.");
                return;
            }

            var po = await _purchaseOrders.GetByIdAsync(poId, ct);
            if (po is null)
            {
                ctx.AddFailure("Purchase order not found.");
                return;
            }

            if (_tenant.FacilityId is long fid && po.FacilityId is not null && po.FacilityId != fid)
                ctx.AddFailure("Purchase order is not in the current facility scope.");

            // Mode 1: SupplierId is optional; it can be inferred later if needed.
            return;
        }

        // Mode 2: Without PO
        if (dto.SupplierId is null)
        {
            ctx.AddFailure("SupplierId is required when PurchaseOrderId is null.");
            return;
        }

        if (dto.SupplierId <= 0)
        {
            ctx.AddFailure("SupplierId must be a positive number.");
            return;
        }

        var supplier = await _suppliers.GetByIdAsync(dto.SupplierId.Value, ct);
        if (supplier is null)
        {
            ctx.AddFailure("Supplier not found.");
            return;
        }
    }
}

