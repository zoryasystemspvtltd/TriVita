using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreateAppointmentStatusHistoryValidator : AbstractValidator<CreateAppointmentStatusHistoryDto>
{
    public CreateAppointmentStatusHistoryValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
