using FluentValidation;
using SharedService.Application.DTOs.Enterprise;

namespace SharedService.Application.Validation;

public sealed class CreateEnterpriseB2BContractDtoValidator : AbstractValidator<CreateEnterpriseB2BContractDto>
{
    public CreateEnterpriseB2BContractDtoValidator()
    {
        RuleFor(x => x.EnterpriseId).GreaterThan(0);
        RuleFor(x => x.PartnerType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PartnerName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ContractCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.TermsJson).MaximumLength(400_000).When(x => x.TermsJson is not null);
        RuleFor(x => x)
            .Must(x => x.EffectiveTo is null || x.EffectiveFrom is null || x.EffectiveTo >= x.EffectiveFrom)
            .WithMessage("EffectiveTo must be on or after EffectiveFrom.");
    }
}

public sealed class UpdateEnterpriseB2BContractDtoValidator : AbstractValidator<UpdateEnterpriseB2BContractDto>
{
    public UpdateEnterpriseB2BContractDtoValidator()
    {
        RuleFor(x => x.PartnerType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PartnerName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ContractCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.TermsJson).MaximumLength(400_000).When(x => x.TermsJson is not null);
        RuleFor(x => x)
            .Must(x => x.EffectiveTo is null || x.EffectiveFrom is null || x.EffectiveTo >= x.EffectiveFrom)
            .WithMessage("EffectiveTo must be on or after EffectiveFrom.");
    }
}
