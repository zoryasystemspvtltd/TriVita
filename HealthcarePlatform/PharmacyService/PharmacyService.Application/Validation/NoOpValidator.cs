using FluentValidation;

namespace PharmacyService.Application.Validation;

public sealed class NoOpValidator<T> : AbstractValidator<T>
{
}
