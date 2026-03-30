using FluentValidation;
using LMSService.Application.DTOs.Workflow;

namespace LMSService.Application.Validation;

public sealed class CreateLmsLabTestBookingDtoValidator : AbstractValidator<CreateLmsLabTestBookingDto>
{
    public CreateLmsLabTestBookingDtoValidator()
    {
        RuleFor(x => x.PatientId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.CatalogTestId).GreaterThan(0);
            item.RuleFor(i => i.WorkflowStatusReferenceValueId).GreaterThan(0);
        });
    }
}

public sealed class RegisterLmsLabSampleBarcodeDtoValidator : AbstractValidator<RegisterLmsLabSampleBarcodeDto>
{
    public RegisterLmsLabSampleBarcodeDtoValidator()
    {
        RuleFor(x => x.BarcodeValue).NotEmpty().MaximumLength(120);
        RuleFor(x => x.TestBookingItemId).GreaterThan(0);
        RuleFor(x => x.BarcodeStatusReferenceValueId).GreaterThan(0);
    }
}

public sealed class CreateLmsEquipmentTypeDtoValidator : AbstractValidator<CreateLmsEquipmentTypeDto>
{
    public CreateLmsEquipmentTypeDtoValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.TypeName).NotEmpty().MaximumLength(250);
    }
}

public sealed class UpdateLmsEquipmentTypeDtoValidator : AbstractValidator<UpdateLmsEquipmentTypeDto>
{
    public UpdateLmsEquipmentTypeDtoValidator()
    {
        RuleFor(x => x.TypeName).NotEmpty().MaximumLength(250);
    }
}

public sealed class CreateLmsEquipmentFacilityMappingDtoValidator : AbstractValidator<CreateLmsEquipmentFacilityMappingDto>
{
    public CreateLmsEquipmentFacilityMappingDtoValidator()
    {
        RuleFor(x => x.EquipmentFacilityId).GreaterThan(0);
        RuleFor(x => x.EquipmentId).GreaterThan(0);
        RuleFor(x => x.MappedFacilityId).GreaterThan(0);
    }
}

public sealed class CreateLmsCatalogTestDtoValidator : AbstractValidator<CreateLmsCatalogTestDto>
{
    public CreateLmsCatalogTestDtoValidator()
    {
        RuleFor(x => x.TestCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.TestName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.DisciplineReferenceValueId).GreaterThan(0);
    }
}

public sealed class UpdateLmsCatalogTestDtoValidator : AbstractValidator<UpdateLmsCatalogTestDto>
{
    public UpdateLmsCatalogTestDtoValidator()
    {
        RuleFor(x => x.TestName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.DisciplineReferenceValueId).GreaterThan(0);
    }
}
