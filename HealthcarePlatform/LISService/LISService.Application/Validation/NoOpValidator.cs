using FluentValidation;

namespace LISService.Application.Validation;

/// <summary>
/// Default validator when no rules are defined; allows optional <see cref="IValidator{T}"/> injection for generated CRUD services.
/// </summary>
public sealed class NoOpValidator<T> : AbstractValidator<T>
{
}
