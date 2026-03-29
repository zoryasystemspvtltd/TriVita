using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Extended;
using HMSService.Application.DTOs.VisitTypes;

namespace HMSService.Application.Services.Extended;

public interface IAppointmentStatusHistoryService
{
    Task<BaseResponse<AppointmentStatusHistoryResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<AppointmentStatusHistoryResponseDto>>> GetPagedAsync(PagedQuery query, long? appointmentId, CancellationToken cancellationToken = default);
    Task<BaseResponse<AppointmentStatusHistoryResponseDto>> CreateAsync(CreateAppointmentStatusHistoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<AppointmentStatusHistoryResponseDto>> UpdateAsync(long id, UpdateAppointmentStatusHistoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IAppointmentQueueService
{
    Task<BaseResponse<AppointmentQueueResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<AppointmentQueueResponseDto>>> GetPagedAsync(PagedQuery query, long? appointmentId, CancellationToken cancellationToken = default);
    Task<BaseResponse<AppointmentQueueResponseDto>> CreateAsync(CreateAppointmentQueueDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<AppointmentQueueResponseDto>> UpdateAsync(long id, UpdateAppointmentQueueDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IVitalService
{
    Task<BaseResponse<VitalResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<VitalResponseDto>>> GetPagedAsync(PagedQuery query, long? visitId, CancellationToken cancellationToken = default);
    Task<BaseResponse<VitalResponseDto>> CreateAsync(CreateVitalDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<VitalResponseDto>> UpdateAsync(long id, UpdateVitalDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IClinicalNoteService
{
    Task<BaseResponse<ClinicalNoteResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ClinicalNoteResponseDto>>> GetPagedAsync(PagedQuery query, long? visitId, CancellationToken cancellationToken = default);
    Task<BaseResponse<ClinicalNoteResponseDto>> CreateAsync(CreateClinicalNoteDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ClinicalNoteResponseDto>> UpdateAsync(long id, UpdateClinicalNoteDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IDiagnosisService
{
    Task<BaseResponse<DiagnosisResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<DiagnosisResponseDto>>> GetPagedAsync(PagedQuery query, long? visitId, CancellationToken cancellationToken = default);
    Task<BaseResponse<DiagnosisResponseDto>> CreateAsync(CreateDiagnosisDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<DiagnosisResponseDto>> UpdateAsync(long id, UpdateDiagnosisDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IMedicalProcedureService
{
    Task<BaseResponse<MedicalProcedureResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<MedicalProcedureResponseDto>>> GetPagedAsync(PagedQuery query, long? visitId, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicalProcedureResponseDto>> CreateAsync(CreateMedicalProcedureDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicalProcedureResponseDto>> UpdateAsync(long id, UpdateMedicalProcedureDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPrescriptionService
{
    Task<BaseResponse<PrescriptionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PrescriptionResponseDto>>> GetPagedAsync(PagedQuery query, long? visitId, long? patientId, CancellationToken cancellationToken = default);
    Task<BaseResponse<PrescriptionResponseDto>> CreateAsync(CreatePrescriptionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PrescriptionResponseDto>> UpdateAsync(long id, UpdatePrescriptionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPrescriptionItemService
{
    Task<BaseResponse<PrescriptionItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PrescriptionItemResponseDto>>> GetPagedAsync(PagedQuery query, long? prescriptionId, CancellationToken cancellationToken = default);
    Task<BaseResponse<PrescriptionItemResponseDto>> CreateAsync(CreatePrescriptionItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PrescriptionItemResponseDto>> UpdateAsync(long id, UpdatePrescriptionItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPrescriptionNoteService
{
    Task<BaseResponse<PrescriptionNoteResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PrescriptionNoteResponseDto>>> GetPagedAsync(PagedQuery query, long? prescriptionId, CancellationToken cancellationToken = default);
    Task<BaseResponse<PrescriptionNoteResponseDto>> CreateAsync(CreatePrescriptionNoteDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PrescriptionNoteResponseDto>> UpdateAsync(long id, UpdatePrescriptionNoteDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPaymentModeService
{
    Task<BaseResponse<PaymentModeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PaymentModeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PaymentModeResponseDto>> CreateAsync(CreatePaymentModeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PaymentModeResponseDto>> UpdateAsync(long id, UpdatePaymentModeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IBillingHeaderService
{
    Task<BaseResponse<BillingHeaderResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<BillingHeaderResponseDto>>> GetPagedAsync(PagedQuery query, long? visitId, long? patientId, CancellationToken cancellationToken = default);
    Task<BaseResponse<BillingHeaderResponseDto>> CreateAsync(CreateBillingHeaderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<BillingHeaderResponseDto>> UpdateAsync(long id, UpdateBillingHeaderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IBillingItemService
{
    Task<BaseResponse<BillingItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<BillingItemResponseDto>>> GetPagedAsync(PagedQuery query, long? billingHeaderId, CancellationToken cancellationToken = default);
    Task<BaseResponse<BillingItemResponseDto>> CreateAsync(CreateBillingItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<BillingItemResponseDto>> UpdateAsync(long id, UpdateBillingItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IPaymentTransactionService
{
    Task<BaseResponse<PaymentTransactionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PaymentTransactionResponseDto>>> GetPagedAsync(PagedQuery query, long? billingHeaderId, CancellationToken cancellationToken = default);
    Task<BaseResponse<PaymentTransactionResponseDto>> CreateAsync(CreatePaymentTransactionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PaymentTransactionResponseDto>> UpdateAsync(long id, UpdatePaymentTransactionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public interface IVisitTypeService
{
    Task<BaseResponse<VisitTypeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<VisitTypeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<VisitTypeResponseDto>> CreateAsync(CreateVisitTypeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<VisitTypeResponseDto>> UpdateAsync(long id, UpdateVisitTypeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
