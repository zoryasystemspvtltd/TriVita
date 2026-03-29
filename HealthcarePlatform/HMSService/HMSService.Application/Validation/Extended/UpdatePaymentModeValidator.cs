using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdatePaymentModeValidator : AbstractValidator<UpdatePaymentModeDto>
{
    public UpdatePaymentModeValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
