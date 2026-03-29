using FluentValidation;
using SharedService.Application.DTOs.FeatureExtensions;

namespace SharedService.Application.Validation;

public sealed class CreateFacilityServicePriceListDtoValidator : AbstractValidator<CreateFacilityServicePriceListDto>
{
    public CreateFacilityServicePriceListDtoValidator()
    {
        RuleFor(x => x.FacilityId).GreaterThan(0);
        RuleFor(x => x.PriceListCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.PriceListName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ServiceModule).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PartnerReferenceCode).MaximumLength(80).When(x => x.PartnerReferenceCode is not null);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x)
            .Must(x => x.EffectiveTo is null || x.EffectiveFrom is null || x.EffectiveTo >= x.EffectiveFrom)
            .WithMessage("EffectiveTo must be on or after EffectiveFrom.");
    }
}

public sealed class UpdateFacilityServicePriceListDtoValidator : AbstractValidator<UpdateFacilityServicePriceListDto>
{
    public UpdateFacilityServicePriceListDtoValidator()
    {
        RuleFor(x => x.PriceListName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ServiceModule).NotEmpty().MaximumLength(30);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x)
            .Must(x => x.EffectiveTo is null || x.EffectiveFrom is null || x.EffectiveTo >= x.EffectiveFrom)
            .WithMessage("EffectiveTo must be on or after EffectiveFrom.");
    }
}

public sealed class CreateFacilityServicePriceListLineDtoValidator : AbstractValidator<CreateFacilityServicePriceListLineDto>
{
    public CreateFacilityServicePriceListLineDtoValidator()
    {
        RuleFor(x => x.FacilityId).GreaterThan(0);
        RuleFor(x => x.PriceListId).GreaterThan(0);
        RuleFor(x => x.ServiceItemCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.ServiceItemName).MaximumLength(250).When(x => x.ServiceItemName is not null);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TaxCategoryCode).MaximumLength(40).When(x => x.TaxCategoryCode is not null);
    }
}

public sealed class UpdateFacilityServicePriceListLineDtoValidator : AbstractValidator<UpdateFacilityServicePriceListLineDto>
{
    public UpdateFacilityServicePriceListLineDtoValidator()
    {
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TaxCategoryCode).MaximumLength(40).When(x => x.TaxCategoryCode is not null);
    }
}

public sealed class CreateCrossFacilityReportAuditDtoValidator : AbstractValidator<CreateCrossFacilityReportAuditDto>
{
    public CreateCrossFacilityReportAuditDtoValidator()
    {
        RuleFor(x => x.ReportCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.ReportName).MaximumLength(250).When(x => x.ReportName is not null);
    }
}

public sealed class UpdateCrossFacilityReportAuditDtoValidator : AbstractValidator<UpdateCrossFacilityReportAuditDto>
{
    public UpdateCrossFacilityReportAuditDtoValidator()
    {
        RuleFor(x => x.ReportName).MaximumLength(250).When(x => x.ReportName is not null);
    }
}

public sealed class CreateModuleIntegrationHandoffDtoValidator : AbstractValidator<CreateModuleIntegrationHandoffDto>
{
    public CreateModuleIntegrationHandoffDtoValidator()
    {
        RuleFor(x => x.CorrelationId).NotEmpty().MaximumLength(64);
        RuleFor(x => x.SourceModule).NotEmpty().MaximumLength(30);
        RuleFor(x => x.TargetModule).NotEmpty().MaximumLength(30);
        RuleFor(x => x.EntityType).NotEmpty().MaximumLength(80);
        RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(40);
    }
}

public sealed class UpdateModuleIntegrationHandoffDtoValidator : AbstractValidator<UpdateModuleIntegrationHandoffDto>
{
    public UpdateModuleIntegrationHandoffDtoValidator()
    {
        RuleFor(x => x.StatusCode).NotEmpty().MaximumLength(40);
    }
}

public sealed class UpsertTenantOnboardingStageDtoValidator : AbstractValidator<UpsertTenantOnboardingStageDto>
{
    public UpsertTenantOnboardingStageDtoValidator()
    {
        RuleFor(x => x.StageCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.StageStatus).NotEmpty().MaximumLength(40);
    }
}

public sealed class CreateLabCriticalValueEscalationDtoValidator : AbstractValidator<CreateLabCriticalValueEscalationDto>
{
    public CreateLabCriticalValueEscalationDtoValidator()
    {
        RuleFor(x => x.FacilityId).GreaterThan(0);
        RuleFor(x => x.EscalationLevel).GreaterThanOrEqualTo(1);
        RuleFor(x => x.ChannelCode).NotEmpty().MaximumLength(40);
        RuleFor(x => x.RecipientSummary).MaximumLength(500).When(x => x.RecipientSummary is not null);
    }
}

public sealed class UpdateLabCriticalValueEscalationDtoValidator : AbstractValidator<UpdateLabCriticalValueEscalationDto>
{
    public UpdateLabCriticalValueEscalationDtoValidator()
    {
        RuleFor(x => x.OutcomeCode).MaximumLength(40).When(x => x.OutcomeCode is not null);
    }
}
