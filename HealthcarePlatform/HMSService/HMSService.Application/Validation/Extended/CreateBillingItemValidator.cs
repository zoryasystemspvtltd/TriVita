using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreateBillingItemValidator : AbstractValidator<CreateBillingItemDto>
{
    public CreateBillingItemValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
