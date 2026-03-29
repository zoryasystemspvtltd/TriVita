using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreateDiagnosisValidator : AbstractValidator<CreateDiagnosisDto>
{
    public CreateDiagnosisValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
