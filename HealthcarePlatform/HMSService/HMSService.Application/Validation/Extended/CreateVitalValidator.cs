using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreateVitalValidator : AbstractValidator<CreateVitalDto>
{
    public CreateVitalValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
