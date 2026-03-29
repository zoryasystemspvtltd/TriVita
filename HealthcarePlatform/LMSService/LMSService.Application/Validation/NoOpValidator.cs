using FluentValidation;

namespace LMSService.Application.Validation;

public sealed class NoOpValidator<T> : AbstractValidator<T>
{
}
