using FluentValidation;
using PharmacyService.Application.DTOs.Entities;

namespace PharmacyService.Application.Validation;

public sealed class UpdatePurchaseBillDtoValidator : AbstractValidator<UpdatePurchaseBillDto>
{
    public UpdatePurchaseBillDtoValidator()
    {
        RuleFor(x => x.InvoiceNo).NotEmpty().MaximumLength(120);
        RuleFor(x => x.InvoiceDate).NotEmpty();
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.GstPercent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OtherTaxAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Items).NotEmpty();
    }
}
