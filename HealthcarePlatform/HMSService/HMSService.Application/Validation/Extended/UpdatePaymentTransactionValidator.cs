using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdatePaymentTransactionValidator : AbstractValidator<UpdatePaymentTransactionDto>
{
    public UpdatePaymentTransactionValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
