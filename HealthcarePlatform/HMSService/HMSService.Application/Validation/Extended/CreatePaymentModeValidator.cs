using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreatePaymentModeValidator : AbstractValidator<CreatePaymentModeDto>
{
    public CreatePaymentModeValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
