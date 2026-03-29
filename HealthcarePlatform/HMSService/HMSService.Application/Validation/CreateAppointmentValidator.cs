using FluentValidation;
using HMSService.Application.DTOs.Appointments;

namespace HMSService.Application.Validation;

public sealed class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.DoctorId).GreaterThan(0);
        RuleFor(x => x.DepartmentId).GreaterThan(0);
        RuleFor(x => x.AppointmentStatusValueId).GreaterThan(0);
        RuleFor(x => x.ScheduledStartOn).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.EffectiveTo is null || x.EffectiveFrom is null || x.EffectiveTo >= x.EffectiveFrom)
            .WithMessage("EffectiveTo must be greater than or equal to EffectiveFrom.");
    }
}
