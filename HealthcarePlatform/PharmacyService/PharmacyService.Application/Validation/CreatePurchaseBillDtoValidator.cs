using FluentValidation;
using PharmacyService.Application.DTOs.Entities;

namespace PharmacyService.Application.Validation;

public sealed class CreatePurchaseBillDtoValidator : AbstractValidator<CreatePurchaseBillDto>
{
    public CreatePurchaseBillDtoValidator()
    {
        RuleFor(x => x.GoodsReceiptId).GreaterThan(0);
        RuleFor(x => x.SupplierId).GreaterThan(0);
        RuleFor(x => x.InvoiceNo).NotEmpty().MaximumLength(120);
        RuleFor(x => x.InvoiceDate).NotEmpty();
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.GstPercent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OtherTaxAmount).GreaterThanOrEqualTo(0);
    }
}
