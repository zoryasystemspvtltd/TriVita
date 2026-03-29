using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreateAppointmentQueueValidator : AbstractValidator<CreateAppointmentQueueDto>
{
    public CreateAppointmentQueueValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
