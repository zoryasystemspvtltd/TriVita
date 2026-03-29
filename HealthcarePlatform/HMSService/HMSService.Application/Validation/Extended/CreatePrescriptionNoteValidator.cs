using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class CreatePrescriptionNoteValidator : AbstractValidator<CreatePrescriptionNoteDto>
{
    public CreatePrescriptionNoteValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
