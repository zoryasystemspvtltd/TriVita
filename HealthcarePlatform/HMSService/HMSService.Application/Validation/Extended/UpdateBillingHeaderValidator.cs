using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdateBillingHeaderValidator : AbstractValidator<UpdateBillingHeaderDto>
{
    public UpdateBillingHeaderValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
