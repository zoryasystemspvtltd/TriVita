using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdatePrescriptionValidator : AbstractValidator<UpdatePrescriptionDto>
{
    public UpdatePrescriptionValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
