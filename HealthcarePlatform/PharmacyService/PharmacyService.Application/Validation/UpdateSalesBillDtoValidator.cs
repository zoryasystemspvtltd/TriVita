using FluentValidation;
using PharmacyService.Application.DTOs.Entities;

namespace PharmacyService.Application.Validation;

public sealed class UpdateSalesBillDtoValidator : AbstractValidator<UpdateSalesBillDto>
{
    public UpdateSalesBillDtoValidator()
    {
        RuleFor(x => x.SalesDate).NotEmpty();
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.GstPercent).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OtherTaxAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(line =>
        {
            line.RuleFor(l => l.MedicineId).GreaterThan(0);
            line.RuleFor(l => l.Quantity).GreaterThan(0);
            line.RuleFor(l => l.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}
