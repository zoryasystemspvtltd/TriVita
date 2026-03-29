using FluentValidation;
using HMSService.Application.DTOs.Extended;

namespace HMSService.Application.Validation.Extended;

public sealed class UpdatePrescriptionNoteValidator : AbstractValidator<UpdatePrescriptionNoteDto>
{
    public UpdatePrescriptionNoteValidator()
    {
        // Minimal rules; extend per business rules.
    }
}
