using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdateAppointmentQueueValidator : AbstractValidator<UpdateAppointmentQueueDto>
{
    public UpdateAppointmentQueueValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
