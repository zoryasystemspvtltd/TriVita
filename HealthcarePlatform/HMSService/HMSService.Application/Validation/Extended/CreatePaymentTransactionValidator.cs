using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreatePaymentTransactionValidator : AbstractValidator<CreatePaymentTransactionDto>
{
    public CreatePaymentTransactionValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
