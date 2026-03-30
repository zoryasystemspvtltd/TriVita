using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Gap;

namespace HMSService.Application.Services.Gap;

public interface IPatientMasterService
{
    Task<BaseResponse<PatientMasterResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<PagedResponse<PatientMasterResponseDto>>> SearchPagedAsync(
        PagedQuery query,
        string? search,
        long? linkedFacilityId,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<PatientMasterResponseDto>> CreateAsync(CreatePatientMasterDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<PatientMasterResponseDto>> UpdateAsync(long id, UpdatePatientMasterDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<PatientMasterResponseDto>> LinkFacilityAsync(LinkPatientFacilityDto dto, CancellationToken cancellationToken = default);
}

public interface IAdmissionWorkflowService
{
    Task<BaseResponse<AdmissionResponseDto>> AdmitAsync(AdmitPatientDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<AdmissionResponseDto>> TransferAsync(TransferPatientDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<AdmissionResponseDto>> DischargeAsync(DischargePatientDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<AdmissionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<PagedResponse<AdmissionResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? patientMasterId,
        CancellationToken cancellationToken = default);
}

public interface IWardService
{
    Task<BaseResponse<WardResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<WardResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<WardResponseDto>> CreateAsync(CreateWardDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<WardResponseDto>> UpdateAsync(long id, UpdateWardDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IBedService
{
    Task<BaseResponse<BedResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<BedResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? wardId,
        bool? onlyAvailable,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<BedResponseDto>> CreateAsync(CreateBedDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<BedResponseDto>> UpdateAsync(long id, UpdateBedDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IHousekeepingStatusService
{
    Task<BaseResponse<HousekeepingStatusResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<HousekeepingStatusResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? bedId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<HousekeepingStatusResponseDto>> CreateAsync(CreateHousekeepingStatusDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<HousekeepingStatusResponseDto>> UpdateAsync(long id, UpdateHousekeepingStatusDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IEmarEntryService
{
    Task<BaseResponse<EmarEntryResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<EmarEntryResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? admissionId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<EmarEntryResponseDto>> CreateAsync(CreateEmarEntryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<EmarEntryResponseDto>> UpdateAsync(long id, UpdateEmarEntryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IDoctorOrderAlertService
{
    Task<BaseResponse<DoctorOrderAlertResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<DoctorOrderAlertResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? visitId,
        long? admissionId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<DoctorOrderAlertResponseDto>> CreateAsync(CreateDoctorOrderAlertDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<DoctorOrderAlertResponseDto>> AcknowledgeAsync(long id, AcknowledgeDoctorOrderAlertDto dto, CancellationToken cancellationToken = default);
}

public interface IOperationTheatreService
{
    Task<BaseResponse<OperationTheatreResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<OperationTheatreResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<OperationTheatreResponseDto>> CreateAsync(CreateOperationTheatreDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<OperationTheatreResponseDto>> UpdateAsync(long id, UpdateOperationTheatreDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface ISurgeryScheduleService
{
    Task<BaseResponse<SurgeryScheduleResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SurgeryScheduleResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? operationTheatreId,
        long? patientMasterId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<SurgeryScheduleResponseDto>> CreateAsync(CreateSurgeryScheduleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SurgeryScheduleResponseDto>> UpdateAsync(long id, UpdateSurgeryScheduleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IAnesthesiaRecordService
{
    Task<BaseResponse<AnesthesiaRecordResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<AnesthesiaRecordResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? surgeryScheduleId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<AnesthesiaRecordResponseDto>> CreateAsync(CreateAnesthesiaRecordDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<AnesthesiaRecordResponseDto>> UpdateAsync(long id, UpdateAnesthesiaRecordDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPostOpRecordService
{
    Task<BaseResponse<PostOpRecordResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PostOpRecordResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? surgeryScheduleId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<PostOpRecordResponseDto>> CreateAsync(CreatePostOpRecordDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PostOpRecordResponseDto>> UpdateAsync(long id, UpdatePostOpRecordDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IOtConsumableService
{
    Task<BaseResponse<OtConsumableResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<OtConsumableResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? surgeryScheduleId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<OtConsumableResponseDto>> CreateAsync(CreateOtConsumableDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<OtConsumableResponseDto>> UpdateAsync(long id, UpdateOtConsumableDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPricingRuleService
{
    Task<BaseResponse<PricingRuleResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PricingRuleResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PricingRuleResponseDto>> CreateAsync(CreatePricingRuleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PricingRuleResponseDto>> UpdateAsync(long id, UpdatePricingRuleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPackageDefinitionService
{
    Task<BaseResponse<PackageDefinitionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PackageDefinitionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PackageDefinitionResponseDto>> CreateAsync(CreatePackageDefinitionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PackageDefinitionResponseDto>> UpdateAsync(long id, UpdatePackageDefinitionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPackageDefinitionLineService
{
    Task<BaseResponse<PackageDefinitionLineResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PackageDefinitionLineResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? packageDefinitionId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<PackageDefinitionLineResponseDto>> CreateAsync(CreatePackageDefinitionLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PackageDefinitionLineResponseDto>> UpdateAsync(long id, UpdatePackageDefinitionLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IProformaInvoiceService
{
    Task<BaseResponse<ProformaInvoiceResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ProformaInvoiceResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ProformaInvoiceResponseDto>> CreateAsync(CreateProformaInvoiceDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ProformaInvoiceResponseDto>> UpdateAsync(long id, UpdateProformaInvoiceDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IInsuranceProviderService
{
    Task<BaseResponse<InsuranceProviderResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<InsuranceProviderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<InsuranceProviderResponseDto>> CreateAsync(CreateInsuranceProviderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<InsuranceProviderResponseDto>> UpdateAsync(long id, UpdateInsuranceProviderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPreAuthorizationService
{
    Task<BaseResponse<PreAuthorizationResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PreAuthorizationResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? insuranceProviderId,
        long? patientMasterId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<PreAuthorizationResponseDto>> CreateAsync(CreatePreAuthorizationDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PreAuthorizationResponseDto>> UpdateAsync(long id, UpdatePreAuthorizationDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IClaimService
{
    Task<BaseResponse<ClaimResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ClaimResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? insuranceProviderId,
        long? patientMasterId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<ClaimResponseDto>> CreateAsync(CreateClaimDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ClaimResponseDto>> UpdateAsync(long id, UpdateClaimDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
