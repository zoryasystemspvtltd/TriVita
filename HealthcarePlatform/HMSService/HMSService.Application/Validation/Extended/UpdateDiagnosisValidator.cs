using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdateDiagnosisValidator : AbstractValidator<UpdateDiagnosisDto>
{
    public UpdateDiagnosisValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
