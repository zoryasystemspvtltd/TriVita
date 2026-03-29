using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreateClinicalNoteValidator : AbstractValidator<CreateClinicalNoteDto>
{
    public CreateClinicalNoteValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
