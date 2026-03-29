using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreatePrescriptionItemValidator : AbstractValidator<CreatePrescriptionItemDto>
{
    public CreatePrescriptionItemValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
