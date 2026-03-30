using FluentValidation;
using HMSService.Application.DTOs.Gap;

namespace HMSService.Application.Validators.Gap;

public sealed class CreatePatientMasterDtoValidator : AbstractValidator<CreatePatientMasterDto>
{
    public CreatePatientMasterDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.PrimaryPhone).MaximumLength(40).When(x => x.PrimaryPhone is not null);
        RuleFor(x => x.PrimaryEmail).MaximumLength(200).When(x => x.PrimaryEmail is not null);
    }
}

public sealed class UpdatePatientMasterDtoValidator : AbstractValidator<UpdatePatientMasterDto>
{
    public UpdatePatientMasterDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.PrimaryPhone).MaximumLength(40).When(x => x.PrimaryPhone is not null);
        RuleFor(x => x.PrimaryEmail).MaximumLength(200).When(x => x.PrimaryEmail is not null);
    }
}

public sealed class LinkPatientFacilityDtoValidator : AbstractValidator<LinkPatientFacilityDto>
{
    public LinkPatientFacilityDtoValidator()
    {
        RuleFor(x => x.PatientMasterId).GreaterThan(0);
        RuleFor(x => x.FacilityId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}

public sealed class CreateWardDtoValidator : AbstractValidator<CreateWardDto>
{
    public CreateWardDtoValidator()
    {
        RuleFor(x => x.WardCode).NotEmpty().MaximumLength(40);
        RuleFor(x => x.WardName).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateWardDtoValidator : AbstractValidator<UpdateWardDto>
{
    public UpdateWardDtoValidator()
    {
        RuleFor(x => x.WardName).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateBedDtoValidator : AbstractValidator<CreateBedDto>
{
    public CreateBedDtoValidator()
    {
        RuleFor(x => x.WardId).GreaterThan(0);
        RuleFor(x => x.BedCode).NotEmpty().MaximumLength(40);
        RuleFor(x => x.BedOperationalStatusReferenceValueId).GreaterThan(0);
    }
}

public sealed class UpdateBedDtoValidator : AbstractValidator<UpdateBedDto>
{
    public UpdateBedDtoValidator()
    {
        RuleFor(x => x.BedOperationalStatusReferenceValueId).GreaterThan(0);
    }
}

public sealed class AdmitPatientDtoValidator : AbstractValidator<AdmitPatientDto>
{
    public AdmitPatientDtoValidator()
    {
        RuleFor(x => x.PatientMasterId).GreaterThan(0);
        RuleFor(x => x.BedId).GreaterThan(0);
        RuleFor(x => x.AdmissionStatusReferenceValueId).GreaterThan(0);
    }
}

public sealed class TransferPatientDtoValidator : AbstractValidator<TransferPatientDto>
{
    public TransferPatientDtoValidator()
    {
        RuleFor(x => x.AdmissionId).GreaterThan(0);
        RuleFor(x => x.ToBedId).GreaterThan(0);
        RuleFor(x => x.Reason).MaximumLength(500).When(x => x.Reason is not null);
    }
}

public sealed class DischargePatientDtoValidator : AbstractValidator<DischargePatientDto>
{
    public DischargePatientDtoValidator()
    {
        RuleFor(x => x.AdmissionId).GreaterThan(0);
        RuleFor(x => x.AdmissionStatusReferenceValueId).GreaterThan(0);
    }
}

public sealed class CreateHousekeepingStatusDtoValidator : AbstractValidator<CreateHousekeepingStatusDto>
{
    public CreateHousekeepingStatusDtoValidator()
    {
        RuleFor(x => x.BedId).GreaterThan(0);
        RuleFor(x => x.HousekeepingStatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}

public sealed class UpdateHousekeepingStatusDtoValidator : AbstractValidator<UpdateHousekeepingStatusDto>
{
    public UpdateHousekeepingStatusDtoValidator()
    {
        RuleFor(x => x.HousekeepingStatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}

public sealed class CreateEmarEntryDtoValidator : AbstractValidator<CreateEmarEntryDto>
{
    public CreateEmarEntryDtoValidator()
    {
        RuleFor(x => x.AdmissionId).GreaterThan(0);
        RuleFor(x => x.MedicationCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.AdministrationStatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}

public sealed class UpdateEmarEntryDtoValidator : AbstractValidator<UpdateEmarEntryDto>
{
    public UpdateEmarEntryDtoValidator()
    {
        RuleFor(x => x.AdministrationStatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}

public sealed class CreateDoctorOrderAlertDtoValidator : AbstractValidator<CreateDoctorOrderAlertDto>
{
    public CreateDoctorOrderAlertDtoValidator()
    {
        RuleFor(x => x.DoctorId).GreaterThan(0);
        RuleFor(x => x.AlertTypeReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(1000);
        RuleFor(x => x)
            .Must(x => x.VisitId.HasValue || x.AdmissionId.HasValue)
            .WithMessage("Either VisitId or AdmissionId must be set.");
    }
}

public sealed class AcknowledgeDoctorOrderAlertDtoValidator : AbstractValidator<AcknowledgeDoctorOrderAlertDto>
{
    public AcknowledgeDoctorOrderAlertDtoValidator()
    {
        RuleFor(x => x.AcknowledgedOn).NotEqual(default(DateTime));
    }
}

public sealed class CreateOperationTheatreDtoValidator : AbstractValidator<CreateOperationTheatreDto>
{
    public CreateOperationTheatreDtoValidator()
    {
        RuleFor(x => x.TheatreCode).NotEmpty().MaximumLength(40);
        RuleFor(x => x.TheatreName).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateOperationTheatreDtoValidator : AbstractValidator<UpdateOperationTheatreDto>
{
    public UpdateOperationTheatreDtoValidator()
    {
        RuleFor(x => x.TheatreName).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateSurgeryScheduleDtoValidator : AbstractValidator<CreateSurgeryScheduleDto>
{
    public CreateSurgeryScheduleDtoValidator()
    {
        RuleFor(x => x.OperationTheatreId).GreaterThan(0);
        RuleFor(x => x.PatientMasterId).GreaterThan(0);
        RuleFor(x => x.SurgeonDoctorId).GreaterThan(0);
        RuleFor(x => x.ScheduleStatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.ProcedureSummary).MaximumLength(500).When(x => x.ProcedureSummary is not null);
    }
}

public sealed class UpdateSurgeryScheduleDtoValidator : AbstractValidator<UpdateSurgeryScheduleDto>
{
    public UpdateSurgeryScheduleDtoValidator()
    {
        RuleFor(x => x.ScheduleStatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.ProcedureSummary).MaximumLength(500).When(x => x.ProcedureSummary is not null);
    }
}

public sealed class CreateAnesthesiaRecordDtoValidator : AbstractValidator<CreateAnesthesiaRecordDto>
{
    public CreateAnesthesiaRecordDtoValidator()
    {
        RuleFor(x => x.SurgeryScheduleId).GreaterThan(0);
    }
}

public sealed class CreatePostOpRecordDtoValidator : AbstractValidator<CreatePostOpRecordDto>
{
    public CreatePostOpRecordDtoValidator()
    {
        RuleFor(x => x.SurgeryScheduleId).GreaterThan(0);
    }
}

public sealed class CreateOtConsumableDtoValidator : AbstractValidator<CreateOtConsumableDto>
{
    public CreateOtConsumableDtoValidator()
    {
        RuleFor(x => x.SurgeryScheduleId).GreaterThan(0);
        RuleFor(x => x.ItemCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.ItemName).MaximumLength(250).When(x => x.ItemName is not null);
    }
}

public sealed class CreatePricingRuleDtoValidator : AbstractValidator<CreatePricingRuleDto>
{
    public CreatePricingRuleDtoValidator()
    {
        RuleFor(x => x.RuleCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.RuleName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ServiceCode).NotEmpty().MaximumLength(80);
    }
}

public sealed class UpdatePricingRuleDtoValidator : AbstractValidator<UpdatePricingRuleDto>
{
    public UpdatePricingRuleDtoValidator()
    {
        RuleFor(x => x.RuleName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.ServiceCode).NotEmpty().MaximumLength(80);
    }
}

public sealed class CreatePackageDefinitionDtoValidator : AbstractValidator<CreatePackageDefinitionDto>
{
    public CreatePackageDefinitionDtoValidator()
    {
        RuleFor(x => x.PackageCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.PackageName).NotEmpty().MaximumLength(250);
    }
}

public sealed class UpdatePackageDefinitionDtoValidator : AbstractValidator<UpdatePackageDefinitionDto>
{
    public UpdatePackageDefinitionDtoValidator()
    {
        RuleFor(x => x.PackageName).NotEmpty().MaximumLength(250);
    }
}

public sealed class CreatePackageDefinitionLineDtoValidator : AbstractValidator<CreatePackageDefinitionLineDto>
{
    public CreatePackageDefinitionLineDtoValidator()
    {
        RuleFor(x => x.PackageDefinitionId).GreaterThan(0);
        RuleFor(x => x.LineNo).GreaterThan(0);
        RuleFor(x => x.ServiceCode).NotEmpty().MaximumLength(80);
    }
}

public sealed class UpdatePackageDefinitionLineDtoValidator : AbstractValidator<UpdatePackageDefinitionLineDto>
{
    public UpdatePackageDefinitionLineDtoValidator()
    {
        RuleFor(x => x.ServiceCode).NotEmpty().MaximumLength(80);
    }
}

public sealed class CreateProformaInvoiceDtoValidator : AbstractValidator<CreateProformaInvoiceDto>
{
    public CreateProformaInvoiceDtoValidator()
    {
        RuleFor(x => x.StatusReferenceValueId).GreaterThan(0);
    }
}

public sealed class CreateInsuranceProviderDtoValidator : AbstractValidator<CreateInsuranceProviderDto>
{
    public CreateInsuranceProviderDtoValidator()
    {
        RuleFor(x => x.ProviderCode).NotEmpty().MaximumLength(80);
        RuleFor(x => x.ProviderName).NotEmpty().MaximumLength(250);
    }
}

public sealed class UpdateInsuranceProviderDtoValidator : AbstractValidator<UpdateInsuranceProviderDto>
{
    public UpdateInsuranceProviderDtoValidator()
    {
        RuleFor(x => x.ProviderName).NotEmpty().MaximumLength(250);
    }
}

public sealed class CreatePreAuthorizationDtoValidator : AbstractValidator<CreatePreAuthorizationDto>
{
    public CreatePreAuthorizationDtoValidator()
    {
        RuleFor(x => x.InsuranceProviderId).GreaterThan(0);
        RuleFor(x => x.PatientMasterId).GreaterThan(0);
        RuleFor(x => x.StatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(2000).When(x => x.Notes is not null);
    }
}

public sealed class UpdatePreAuthorizationDtoValidator : AbstractValidator<UpdatePreAuthorizationDto>
{
    public UpdatePreAuthorizationDtoValidator()
    {
        RuleFor(x => x.StatusReferenceValueId).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(2000).When(x => x.Notes is not null);
    }
}

public sealed class CreateClaimDtoValidator : AbstractValidator<CreateClaimDto>
{
    public CreateClaimDtoValidator()
    {
        RuleFor(x => x.InsuranceProviderId).GreaterThan(0);
        RuleFor(x => x.PatientMasterId).GreaterThan(0);
        RuleFor(x => x.StatusReferenceValueId).GreaterThan(0);
    }
}

public sealed class UpdateClaimDtoValidator : AbstractValidator<UpdateClaimDto>
{
    public UpdateClaimDtoValidator()
    {
        RuleFor(x => x.StatusReferenceValueId).GreaterThan(0);
    }
}

public sealed class UpdateAnesthesiaRecordDtoValidator : AbstractValidator<UpdateAnesthesiaRecordDto>
{
    public UpdateAnesthesiaRecordDtoValidator()
    {
    }
}

public sealed class UpdatePostOpRecordDtoValidator : AbstractValidator<UpdatePostOpRecordDto>
{
    public UpdatePostOpRecordDtoValidator()
    {
    }
}

public sealed class UpdateOtConsumableDtoValidator : AbstractValidator<UpdateOtConsumableDto>
{
    public UpdateOtConsumableDtoValidator()
    {
    }
}

public sealed class UpdateProformaInvoiceDtoValidator : AbstractValidator<UpdateProformaInvoiceDto>
{
    public UpdateProformaInvoiceDtoValidator()
    {
        RuleFor(x => x.StatusReferenceValueId).GreaterThan(0);
    }
}
