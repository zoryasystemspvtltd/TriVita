using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreateBillingHeaderValidator : AbstractValidator<CreateBillingHeaderDto>
{
    public CreateBillingHeaderValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
