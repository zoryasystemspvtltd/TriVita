using FluentValidation;
using HMSService.Application.DTOs.Visits;

namespace HMSService.Application.Validation;

public sealed class UpdateVisitValidator : AbstractValidator<UpdateVisitDto>
{
    public UpdateVisitValidator()
    {
        RuleFor(x => x.DoctorId).GreaterThan(0);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.VisitTypeId).GreaterThan(0);
        RuleFor(x => x.VisitStartOn).NotEmpty();
        RuleFor(x => x.CurrentStatusReferenceValueId).GreaterThan(0);
    }
}
