using FluentValidation;
using HMSService.Application.DTOs.VisitTypes;

namespace HMSService.Application.Validation.VisitTypes;

public sealed class UpdateVisitTypeValidator : AbstractValidator<UpdateVisitTypeDto>
{
    public UpdateVisitTypeValidator()
    {
        RuleFor(x => x.VisitTypeCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.VisitTypeName).NotEmpty().MaximumLength(250);
    }
}
