using AutoMapper;
using HMSService.Application.DTOs.Appointments;
using HMSService.Application.DTOs.Extended;
using HMSService.Application.DTOs.Gap;
using HMSService.Application.DTOs.VisitTypes;
using HMSService.Application.DTOs.Visits;
using HMSService.Domain.Entities;

namespace HMSService.Application.Mapping;

public sealed class HmsMappingProfile : Profile
{
    public HmsMappingProfile()
    {
        CreateMap<HmsAppointment, AppointmentResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));

        CreateMap<HmsVisit, VisitResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));

        CreateMap<CreateAppointmentDto, HmsAppointment>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.AppointmentNo, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore())
            .ForMember(d => d.VisitType, o => o.Ignore());

        CreateMap<CreateVisitDto, HmsVisit>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.VisitNo, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore())
            .ForMember(d => d.Appointment, o => o.Ignore())
            .ForMember(d => d.VisitType, o => o.Ignore());

        // --- HMS extended (clinical / billing) ---
        CreateMap<HmsAppointmentStatusHistory, AppointmentStatusHistoryResponseDto>();
        CreateMap<CreateAppointmentStatusHistoryDto, HmsAppointmentStatusHistory>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateAppointmentStatusHistoryDto, HmsAppointmentStatusHistory>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsAppointmentQueue, AppointmentQueueResponseDto>();
        CreateMap<CreateAppointmentQueueDto, HmsAppointmentQueue>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateAppointmentQueueDto, HmsAppointmentQueue>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsVital, VitalResponseDto>();
        CreateMap<CreateVitalDto, HmsVital>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateVitalDto, HmsVital>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsClinicalNote, ClinicalNoteResponseDto>();
        CreateMap<CreateClinicalNoteDto, HmsClinicalNote>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateClinicalNoteDto, HmsClinicalNote>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsDiagnosis, DiagnosisResponseDto>();
        CreateMap<CreateDiagnosisDto, HmsDiagnosis>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateDiagnosisDto, HmsDiagnosis>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsMedicalProcedure, MedicalProcedureResponseDto>();
        CreateMap<CreateMedicalProcedureDto, HmsMedicalProcedure>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateMedicalProcedureDto, HmsMedicalProcedure>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsPrescription, PrescriptionResponseDto>();
        CreateMap<CreatePrescriptionDto, HmsPrescription>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdatePrescriptionDto, HmsPrescription>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsPrescriptionItem, PrescriptionItemResponseDto>();
        CreateMap<CreatePrescriptionItemDto, HmsPrescriptionItem>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdatePrescriptionItemDto, HmsPrescriptionItem>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsPrescriptionNote, PrescriptionNoteResponseDto>();
        CreateMap<CreatePrescriptionNoteDto, HmsPrescriptionNote>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdatePrescriptionNoteDto, HmsPrescriptionNote>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsPaymentMode, PaymentModeResponseDto>();
        CreateMap<CreatePaymentModeDto, HmsPaymentMode>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdatePaymentModeDto, HmsPaymentMode>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsBillingHeader, BillingHeaderResponseDto>();
        CreateMap<CreateBillingHeaderDto, HmsBillingHeader>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateBillingHeaderDto, HmsBillingHeader>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsBillingItem, BillingItemResponseDto>();
        CreateMap<CreateBillingItemDto, HmsBillingItem>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateBillingItemDto, HmsBillingItem>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsPaymentTransaction, PaymentTransactionResponseDto>();
        CreateMap<CreatePaymentTransactionDto, HmsPaymentTransaction>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdatePaymentTransactionDto, HmsPaymentTransaction>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsVisitType, VisitTypeResponseDto>();
        CreateMap<CreateVisitTypeDto, HmsVisitType>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateVisitTypeDto, HmsVisitType>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        // --- HMS enterprise gap (patient master / IPD / nursing) ---
        CreateMap<HmsPatientMaster, PatientMasterResponseDto>();
        CreateMap<CreatePatientMasterDto, HmsPatientMaster>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.Upid, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdatePatientMasterDto, HmsPatientMaster>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.Upid, o => o.Ignore())
            .ForMember(d => d.SharedPatientId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsWard, WardResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreateWardDto, HmsWard>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateWardDto, HmsWard>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.WardCode, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsBed, BedResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreateBedDto, HmsBed>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.CurrentAdmissionId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateBedDto, HmsBed>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.WardId, o => o.Ignore())
            .ForMember(d => d.BedCode, o => o.Ignore())
            .ForMember(d => d.BedCategoryReferenceValueId, o => o.Ignore())
            .ForMember(d => d.CurrentAdmissionId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsAdmission, AdmissionResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));

        CreateMap<HmsHousekeepingStatus, HousekeepingStatusResponseDto>();
        CreateMap<CreateHousekeepingStatusDto, HmsHousekeepingStatus>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateHousekeepingStatusDto, HmsHousekeepingStatus>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.BedId, o => o.Ignore())
            .ForMember(d => d.RecordedOn, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsEmarEntry, EmarEntryResponseDto>();
        CreateMap<CreateEmarEntryDto, HmsEmarEntry>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.AdministeredOn, o => o.Ignore())
            .ForMember(d => d.NurseUserId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
        CreateMap<UpdateEmarEntryDto, HmsEmarEntry>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.AdmissionId, o => o.Ignore())
            .ForMember(d => d.MedicationCode, o => o.Ignore())
            .ForMember(d => d.ScheduledOn, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        CreateMap<HmsDoctorOrderAlert, DoctorOrderAlertResponseDto>();
        CreateMap<CreateDoctorOrderAlertDto, HmsDoctorOrderAlert>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.AcknowledgedOn, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

        // --- HMS script 10: OT / billing / insurance ---
        CreateMap<HmsOperationTheatre, OperationTheatreResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreateOperationTheatreDto, HmsOperationTheatre>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdateOperationTheatreDto, HmsOperationTheatre>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.TheatreCode, o => o.Ignore());

        CreateMap<HmsSurgerySchedule, SurgeryScheduleResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreateSurgeryScheduleDto, HmsSurgerySchedule>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdateSurgeryScheduleDto, HmsSurgerySchedule>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.OperationTheatreId, o => o.Ignore())
            .ForMember(d => d.PatientMasterId, o => o.Ignore())
            .ForMember(d => d.SurgeonDoctorId, o => o.Ignore());

        CreateMap<HmsAnesthesiaRecord, AnesthesiaRecordResponseDto>();
        CreateMap<CreateAnesthesiaRecordDto, HmsAnesthesiaRecord>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdateAnesthesiaRecordDto, HmsAnesthesiaRecord>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.SurgeryScheduleId, o => o.Ignore());

        CreateMap<HmsPostOpRecord, PostOpRecordResponseDto>();
        CreateMap<CreatePostOpRecordDto, HmsPostOpRecord>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdatePostOpRecordDto, HmsPostOpRecord>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.SurgeryScheduleId, o => o.Ignore());

        CreateMap<HmsOtConsumable, OtConsumableResponseDto>();
        CreateMap<CreateOtConsumableDto, HmsOtConsumable>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdateOtConsumableDto, HmsOtConsumable>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.SurgeryScheduleId, o => o.Ignore())
            .ForMember(d => d.ItemCode, o => o.Ignore());

        CreateMap<HmsPricingRule, PricingRuleResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreatePricingRuleDto, HmsPricingRule>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdatePricingRuleDto, HmsPricingRule>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.RuleCode, o => o.Ignore());

        CreateMap<HmsPackageDefinition, PackageDefinitionResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreatePackageDefinitionDto, HmsPackageDefinition>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdatePackageDefinitionDto, HmsPackageDefinition>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.PackageCode, o => o.Ignore());

        CreateMap<HmsPackageDefinitionLine, PackageDefinitionLineResponseDto>();
        CreateMap<CreatePackageDefinitionLineDto, HmsPackageDefinitionLine>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdatePackageDefinitionLineDto, HmsPackageDefinitionLine>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.PackageDefinitionId, o => o.Ignore())
            .ForMember(d => d.LineNumber, o => o.Ignore());

        CreateMap<HmsProformaInvoice, ProformaInvoiceResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreateProformaInvoiceDto, HmsProformaInvoice>()
            .ApplyStandardHmsGapIgnores()
            .ForMember(d => d.ProformaNo, o => o.Ignore());
        CreateMap<UpdateProformaInvoiceDto, HmsProformaInvoice>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.ProformaNo, o => o.Ignore())
            .ForMember(d => d.PatientMasterId, o => o.Ignore())
            .ForMember(d => d.VisitId, o => o.Ignore());

        CreateMap<HmsInsuranceProvider, InsuranceProviderResponseDto>();
        CreateMap<CreateInsuranceProviderDto, HmsInsuranceProvider>().ApplyStandardHmsGapIgnores();
        CreateMap<UpdateInsuranceProviderDto, HmsInsuranceProvider>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.ProviderCode, o => o.Ignore());

        CreateMap<HmsPreAuthorization, PreAuthorizationResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreatePreAuthorizationDto, HmsPreAuthorization>()
            .ApplyStandardHmsGapIgnores()
            .ForMember(d => d.PreAuthNo, o => o.Ignore());
        CreateMap<UpdatePreAuthorizationDto, HmsPreAuthorization>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.PreAuthNo, o => o.Ignore())
            .ForMember(d => d.InsuranceProviderId, o => o.Ignore())
            .ForMember(d => d.PatientMasterId, o => o.Ignore())
            .ForMember(d => d.RequestedOn, o => o.Ignore());

        CreateMap<HmsClaim, ClaimResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreateClaimDto, HmsClaim>()
            .ApplyStandardHmsGapIgnores()
            .ForMember(d => d.ClaimNo, o => o.Ignore());
        CreateMap<UpdateClaimDto, HmsClaim>()
            .ApplyStandardHmsGapUpdateIgnores()
            .ForMember(d => d.ClaimNo, o => o.Ignore())
            .ForMember(d => d.InsuranceProviderId, o => o.Ignore())
            .ForMember(d => d.PatientMasterId, o => o.Ignore());
    }
}
