using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdateClinicalNoteValidator : AbstractValidator<UpdateClinicalNoteDto>
{
    public UpdateClinicalNoteValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
