using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdateAppointmentStatusHistoryValidator : AbstractValidator<UpdateAppointmentStatusHistoryDto>
{
    public UpdateAppointmentStatusHistoryValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
