using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdateMedicalProcedureValidator : AbstractValidator<UpdateMedicalProcedureDto>
{
    public UpdateMedicalProcedureValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
