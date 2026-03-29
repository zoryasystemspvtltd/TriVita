using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdateVitalValidator : AbstractValidator<UpdateVitalDto>
{
    public UpdateVitalValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
