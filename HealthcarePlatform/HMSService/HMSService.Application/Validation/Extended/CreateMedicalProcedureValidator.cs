using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreateMedicalProcedureValidator : AbstractValidator<CreateMedicalProcedureDto>
{
    public CreateMedicalProcedureValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
