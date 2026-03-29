using FluentValidation;
using HMSService.Application.DTOs.VisitTypes;

namespace HMSService.Application.Validation.VisitTypes;

public sealed class CreateVisitTypeValidator : AbstractValidator<CreateVisitTypeDto>
{
    public CreateVisitTypeValidator()
    {
        RuleFor(x => x.VisitTypeCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.VisitTypeName).NotEmpty().MaximumLength(250);
    }
}
