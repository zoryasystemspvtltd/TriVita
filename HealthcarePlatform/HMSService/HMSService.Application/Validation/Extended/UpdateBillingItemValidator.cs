using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdateBillingItemValidator : AbstractValidator<UpdateBillingItemDto>
{
    public UpdateBillingItemValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
