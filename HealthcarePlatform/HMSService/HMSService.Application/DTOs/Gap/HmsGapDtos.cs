namespace HMSService.Application.DTOs.Gap;

#region Patient master

public sealed class PatientMasterResponseDto
{
    public long Id { get; init; }
    public string Upid { get; init; } = null!;
    public long? SharedPatientId { get; init; }
    public string FullName { get; init; } = null!;
    public DateTime? DateOfBirth { get; init; }
    public long? GenderReferenceValueId { get; init; }
    public string? PrimaryPhone { get; init; }
    public string? PrimaryEmail { get; init; }
}

public sealed class CreatePatientMasterDto
{
    public long? SharedPatientId { get; init; }
    public string FullName { get; init; } = null!;
    public DateTime? DateOfBirth { get; init; }
    public long? GenderReferenceValueId { get; init; }
    public string? PrimaryPhone { get; init; }
    public string? PrimaryEmail { get; init; }
}

public sealed class UpdatePatientMasterDto
{
    public string FullName { get; init; } = null!;
    public DateTime? DateOfBirth { get; init; }
    public long? GenderReferenceValueId { get; init; }
    public string? PrimaryPhone { get; init; }
    public string? PrimaryEmail { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class LinkPatientFacilityDto
{
    public long PatientMasterId { get; init; }
    public long FacilityId { get; init; }
    public string? Notes { get; init; }
}

#endregion

#region IPD catalog

public sealed class WardResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string WardCode { get; init; } = null!;
    public string WardName { get; init; } = null!;
    public long? WardCategoryReferenceValueId { get; init; }
}

public sealed class CreateWardDto
{
    public string WardCode { get; init; } = null!;
    public string WardName { get; init; } = null!;
    public long? WardCategoryReferenceValueId { get; init; }
}

public sealed class UpdateWardDto
{
    public string WardName { get; init; } = null!;
    public long? WardCategoryReferenceValueId { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class BedResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public long WardId { get; init; }
    public string BedCode { get; init; } = null!;
    public long? BedCategoryReferenceValueId { get; init; }
    public long BedOperationalStatusReferenceValueId { get; init; }
    public long? CurrentAdmissionId { get; init; }
}

public sealed class CreateBedDto
{
    public long WardId { get; init; }
    public string BedCode { get; init; } = null!;
    public long? BedCategoryReferenceValueId { get; init; }
    public long BedOperationalStatusReferenceValueId { get; init; }
}

public sealed class UpdateBedDto
{
    public long BedOperationalStatusReferenceValueId { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Admission workflow

public sealed class AdmissionResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string AdmissionNo { get; init; } = null!;
    public long PatientMasterId { get; init; }
    public long BedId { get; init; }
    public long AdmissionStatusReferenceValueId { get; init; }
    public DateTime AdmittedOn { get; init; }
    public DateTime? DischargedOn { get; init; }
    public long? AttendingDoctorId { get; init; }
}

public sealed class AdmitPatientDto
{
    public long PatientMasterId { get; init; }
    public long BedId { get; init; }
    public long AdmissionStatusReferenceValueId { get; init; }
    public long? AttendingDoctorId { get; init; }
}

public sealed class TransferPatientDto
{
    public long AdmissionId { get; init; }
    public long ToBedId { get; init; }
    public string? Reason { get; init; }
}

public sealed class DischargePatientDto
{
    public long AdmissionId { get; init; }
    public long AdmissionStatusReferenceValueId { get; init; }
}

#endregion

#region Housekeeping / eMAR / alerts

public sealed class HousekeepingStatusResponseDto
{
    public long Id { get; init; }
    public long BedId { get; init; }
    public long HousekeepingStatusReferenceValueId { get; init; }
    public DateTime RecordedOn { get; init; }
    public string? Notes { get; init; }
}

public sealed class CreateHousekeepingStatusDto
{
    public long BedId { get; init; }
    public long HousekeepingStatusReferenceValueId { get; init; }
    public DateTime RecordedOn { get; init; }
    public string? Notes { get; init; }
}

public sealed class UpdateHousekeepingStatusDto
{
    public long HousekeepingStatusReferenceValueId { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class EmarEntryResponseDto
{
    public long Id { get; init; }
    public long AdmissionId { get; init; }
    public string MedicationCode { get; init; } = null!;
    public DateTime ScheduledOn { get; init; }
    public DateTime? AdministeredOn { get; init; }
    public long AdministrationStatusReferenceValueId { get; init; }
    public long? NurseUserId { get; init; }
    public string? Notes { get; init; }
}

public sealed class CreateEmarEntryDto
{
    public long AdmissionId { get; init; }
    public string MedicationCode { get; init; } = null!;
    public DateTime ScheduledOn { get; init; }
    public long AdministrationStatusReferenceValueId { get; init; }
    public string? Notes { get; init; }
}

public sealed class UpdateEmarEntryDto
{
    public DateTime? AdministeredOn { get; init; }
    public long AdministrationStatusReferenceValueId { get; init; }
    public long? NurseUserId { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class DoctorOrderAlertResponseDto
{
    public long Id { get; init; }
    public long? VisitId { get; init; }
    public long? AdmissionId { get; init; }
    public long DoctorId { get; init; }
    public long AlertTypeReferenceValueId { get; init; }
    public string Message { get; init; } = null!;
    public DateTime? AcknowledgedOn { get; init; }
}

public sealed class CreateDoctorOrderAlertDto
{
    public long? VisitId { get; init; }
    public long? AdmissionId { get; init; }
    public long DoctorId { get; init; }
    public long AlertTypeReferenceValueId { get; init; }
    public string Message { get; init; } = null!;
}

public sealed class AcknowledgeDoctorOrderAlertDto
{
    public DateTime AcknowledgedOn { get; init; }
}

#endregion

#region OT

public sealed class OperationTheatreResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string TheatreCode { get; init; } = null!;
    public string TheatreName { get; init; } = null!;
    public long? DepartmentId { get; init; }
}

public sealed class CreateOperationTheatreDto
{
    public string TheatreCode { get; init; } = null!;
    public string TheatreName { get; init; } = null!;
    public long? DepartmentId { get; init; }
}

public sealed class UpdateOperationTheatreDto
{
    public string TheatreName { get; init; } = null!;
    public long? DepartmentId { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class SurgeryScheduleResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public long OperationTheatreId { get; init; }
    public long PatientMasterId { get; init; }
    public long SurgeonDoctorId { get; init; }
    public DateTime ScheduledStartOn { get; init; }
    public DateTime? ScheduledEndOn { get; init; }
    public string? ProcedureSummary { get; init; }
    public long ScheduleStatusReferenceValueId { get; init; }
}

public sealed class CreateSurgeryScheduleDto
{
    public long OperationTheatreId { get; init; }
    public long PatientMasterId { get; init; }
    public long SurgeonDoctorId { get; init; }
    public DateTime ScheduledStartOn { get; init; }
    public DateTime? ScheduledEndOn { get; init; }
    public string? ProcedureSummary { get; init; }
    public long ScheduleStatusReferenceValueId { get; init; }
}

public sealed class UpdateSurgeryScheduleDto
{
    public DateTime ScheduledStartOn { get; init; }
    public DateTime? ScheduledEndOn { get; init; }
    public string? ProcedureSummary { get; init; }
    public long ScheduleStatusReferenceValueId { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class AnesthesiaRecordResponseDto
{
    public long Id { get; init; }
    public long SurgeryScheduleId { get; init; }
    public long? AnesthesiologistDoctorId { get; init; }
    public string? RecordJson { get; init; }
    public DateTime RecordedOn { get; init; }
}

public sealed class CreateAnesthesiaRecordDto
{
    public long SurgeryScheduleId { get; init; }
    public long? AnesthesiologistDoctorId { get; init; }
    public string? RecordJson { get; init; }
    public DateTime RecordedOn { get; init; }
}

public sealed class UpdateAnesthesiaRecordDto
{
    public long? AnesthesiologistDoctorId { get; init; }
    public string? RecordJson { get; init; }
    public DateTime RecordedOn { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PostOpRecordResponseDto
{
    public long Id { get; init; }
    public long SurgeryScheduleId { get; init; }
    public string? RecoveryNotes { get; init; }
    public DateTime RecordedOn { get; init; }
}

public sealed class CreatePostOpRecordDto
{
    public long SurgeryScheduleId { get; init; }
    public string? RecoveryNotes { get; init; }
    public DateTime RecordedOn { get; init; }
}

public sealed class UpdatePostOpRecordDto
{
    public string? RecoveryNotes { get; init; }
    public DateTime RecordedOn { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class OtConsumableResponseDto
{
    public long Id { get; init; }
    public long SurgeryScheduleId { get; init; }
    public string ItemCode { get; init; } = null!;
    public string? ItemName { get; init; }
    public decimal Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    public bool Billable { get; init; }
}

public sealed class CreateOtConsumableDto
{
    public long SurgeryScheduleId { get; init; }
    public string ItemCode { get; init; } = null!;
    public string? ItemName { get; init; }
    public decimal Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    public bool Billable { get; init; } = true;
}

public sealed class UpdateOtConsumableDto
{
    public string? ItemName { get; init; }
    public decimal Quantity { get; init; }
    public decimal? UnitPrice { get; init; }
    public bool Billable { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Billing extensions

public sealed class PricingRuleResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string RuleCode { get; init; } = null!;
    public string RuleName { get; init; } = null!;
    public long? TariffTypeReferenceValueId { get; init; }
    public string ServiceCode { get; init; } = null!;
    public decimal UnitPrice { get; init; }
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
}

public sealed class CreatePricingRuleDto
{
    public string RuleCode { get; init; } = null!;
    public string RuleName { get; init; } = null!;
    public long? TariffTypeReferenceValueId { get; init; }
    public string ServiceCode { get; init; } = null!;
    public decimal UnitPrice { get; init; }
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdatePricingRuleDto
{
    public string RuleName { get; init; } = null!;
    public long? TariffTypeReferenceValueId { get; init; }
    public string ServiceCode { get; init; } = null!;
    public decimal UnitPrice { get; init; }
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PackageDefinitionResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string PackageCode { get; init; } = null!;
    public string PackageName { get; init; } = null!;
    public decimal BundlePrice { get; init; }
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
}

public sealed class CreatePackageDefinitionDto
{
    public string PackageCode { get; init; } = null!;
    public string PackageName { get; init; } = null!;
    public decimal BundlePrice { get; init; }
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdatePackageDefinitionDto
{
    public string PackageName { get; init; } = null!;
    public decimal BundlePrice { get; init; }
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PackageDefinitionLineResponseDto
{
    public long Id { get; init; }
    public long PackageDefinitionId { get; init; }
    public int LineNumber { get; init; }
    public string ServiceCode { get; init; } = null!;
    public decimal Quantity { get; init; }
}

public sealed class CreatePackageDefinitionLineDto
{
    public long PackageDefinitionId { get; init; }
    public int LineNumber { get; init; }
    public string ServiceCode { get; init; } = null!;
    public decimal Quantity { get; init; }
}

public sealed class UpdatePackageDefinitionLineDto
{
    public string ServiceCode { get; init; } = null!;
    public decimal Quantity { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class ProformaInvoiceResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string ProformaNo { get; init; } = null!;
    public long? PatientMasterId { get; init; }
    public long? VisitId { get; init; }
    public decimal GrandTotal { get; init; }
    public long StatusReferenceValueId { get; init; }
    public string? LinesJson { get; init; }
}

public sealed class CreateProformaInvoiceDto
{
    public long? PatientMasterId { get; init; }
    public long? VisitId { get; init; }
    public decimal GrandTotal { get; init; }
    public long StatusReferenceValueId { get; init; }
    public string? LinesJson { get; init; }
}

public sealed class UpdateProformaInvoiceDto
{
    public decimal GrandTotal { get; init; }
    public long StatusReferenceValueId { get; init; }
    public string? LinesJson { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Insurance / TPA

public sealed class InsuranceProviderResponseDto
{
    public long Id { get; init; }
    public long? FacilityId { get; init; }
    public string ProviderCode { get; init; } = null!;
    public string ProviderName { get; init; } = null!;
    public long? TpaCategoryReferenceValueId { get; init; }
}

public sealed class CreateInsuranceProviderDto
{
    public string ProviderCode { get; init; } = null!;
    public string ProviderName { get; init; } = null!;
    public long? TpaCategoryReferenceValueId { get; init; }
}

public sealed class UpdateInsuranceProviderDto
{
    public string ProviderName { get; init; } = null!;
    public long? TpaCategoryReferenceValueId { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PreAuthorizationResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string PreAuthNo { get; init; } = null!;
    public long InsuranceProviderId { get; init; }
    public long PatientMasterId { get; init; }
    public DateTime RequestedOn { get; init; }
    public long StatusReferenceValueId { get; init; }
    public decimal? ApprovedAmount { get; init; }
    public string? Notes { get; init; }
}

public sealed class CreatePreAuthorizationDto
{
    public long InsuranceProviderId { get; init; }
    public long PatientMasterId { get; init; }
    public DateTime RequestedOn { get; init; }
    public long StatusReferenceValueId { get; init; }
    public decimal? ApprovedAmount { get; init; }
    public string? Notes { get; init; }
}

public sealed class UpdatePreAuthorizationDto
{
    public long StatusReferenceValueId { get; init; }
    public decimal? ApprovedAmount { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class ClaimResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string ClaimNo { get; init; } = null!;
    public long InsuranceProviderId { get; init; }
    public long PatientMasterId { get; init; }
    public long? BillingHeaderId { get; init; }
    public DateTime? SubmittedOn { get; init; }
    public long StatusReferenceValueId { get; init; }
    public decimal? ClaimAmount { get; init; }
}

public sealed class CreateClaimDto
{
    public long InsuranceProviderId { get; init; }
    public long PatientMasterId { get; init; }
    public long? BillingHeaderId { get; init; }
    public DateTime? SubmittedOn { get; init; }
    public long StatusReferenceValueId { get; init; }
    public decimal? ClaimAmount { get; init; }
}

public sealed class UpdateClaimDto
{
    public long? BillingHeaderId { get; init; }
    public DateTime? SubmittedOn { get; init; }
    public long StatusReferenceValueId { get; init; }
    public decimal? ClaimAmount { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion
