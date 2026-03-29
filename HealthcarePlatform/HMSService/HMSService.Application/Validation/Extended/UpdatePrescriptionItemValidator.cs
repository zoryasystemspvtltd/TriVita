using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdatePrescriptionItemValidator : AbstractValidator<UpdatePrescriptionItemDto>
{
    public UpdatePrescriptionItemValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
