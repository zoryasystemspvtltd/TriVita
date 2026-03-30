using FluentValidation;
using LISService.Application.DTOs.Analyzer;

namespace LISService.Application.Validation;

public sealed class AnalyzerResultIngestDtoValidator : AbstractValidator<AnalyzerResultIngestDto>
{
    public AnalyzerResultIngestDtoValidator()
    {
        RuleFor(x => x.Barcode).NotEmpty().MaximumLength(120);
        RuleFor(x => x.EquipmentTestCode).NotEmpty().MaximumLength(120);
        RuleFor(x => x.ResultHeaderStatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.ResultLineStatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Values).NotEmpty();
    }
}
