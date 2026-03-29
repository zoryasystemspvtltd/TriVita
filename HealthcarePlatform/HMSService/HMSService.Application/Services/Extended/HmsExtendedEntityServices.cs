using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Extended;
using HMSService.Application.DTOs.VisitTypes;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HMSService.Application.Services.Extended;

public sealed class AppointmentStatusHistoryService
    : HmsCrudServiceBase<HmsAppointmentStatusHistory, CreateAppointmentStatusHistoryDto, UpdateAppointmentStatusHistoryDto, AppointmentStatusHistoryResponseDto, AppointmentStatusHistoryService>,
        IAppointmentStatusHistoryService
{
    public AppointmentStatusHistoryService(
        IRepository<HmsAppointmentStatusHistory> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateAppointmentStatusHistoryDto>? createValidator,
        IValidator<UpdateAppointmentStatusHistoryDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<AppointmentStatusHistoryService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<AppointmentStatusHistoryResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? appointmentId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            appointmentId is null ? null : e => e.AppointmentId == appointmentId.Value,
            cancellationToken);
}

public sealed class AppointmentQueueService
    : HmsCrudServiceBase<HmsAppointmentQueue, CreateAppointmentQueueDto, UpdateAppointmentQueueDto, AppointmentQueueResponseDto, AppointmentQueueService>,
        IAppointmentQueueService
{
    public AppointmentQueueService(
        IRepository<HmsAppointmentQueue> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateAppointmentQueueDto>? createValidator,
        IValidator<UpdateAppointmentQueueDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<AppointmentQueueService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override async Task OnBeforeCreateAsync(
        HmsAppointmentQueue entity,
        CreateAppointmentQueueDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity.QueueToken))
            entity.QueueToken = $"Q-{Guid.NewGuid():N}"[..8].ToUpperInvariant();
        await Task.CompletedTask;
    }

    public Task<BaseResponse<PagedResponse<AppointmentQueueResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? appointmentId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            appointmentId is null ? null : e => e.AppointmentId == appointmentId.Value,
            cancellationToken);
}

public sealed class VitalService
    : HmsCrudServiceBase<HmsVital, CreateVitalDto, UpdateVitalDto, VitalResponseDto, VitalService>,
        IVitalService
{
    public VitalService(
        IRepository<HmsVital> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateVitalDto>? createValidator,
        IValidator<UpdateVitalDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<VitalService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<VitalResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? visitId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            visitId is null ? null : e => e.VisitId == visitId.Value,
            cancellationToken);
}

public sealed class ClinicalNoteService
    : HmsCrudServiceBase<HmsClinicalNote, CreateClinicalNoteDto, UpdateClinicalNoteDto, ClinicalNoteResponseDto, ClinicalNoteService>,
        IClinicalNoteService
{
    public ClinicalNoteService(
        IRepository<HmsClinicalNote> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateClinicalNoteDto>? createValidator,
        IValidator<UpdateClinicalNoteDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<ClinicalNoteService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<ClinicalNoteResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? visitId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            visitId is null ? null : e => e.VisitId == visitId.Value,
            cancellationToken);
}

public sealed class DiagnosisService
    : HmsCrudServiceBase<HmsDiagnosis, CreateDiagnosisDto, UpdateDiagnosisDto, DiagnosisResponseDto, DiagnosisService>,
        IDiagnosisService
{
    public DiagnosisService(
        IRepository<HmsDiagnosis> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateDiagnosisDto>? createValidator,
        IValidator<UpdateDiagnosisDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<DiagnosisService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<DiagnosisResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? visitId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            visitId is null ? null : e => e.VisitId == visitId.Value,
            cancellationToken);
}

public sealed class MedicalProcedureService
    : HmsCrudServiceBase<HmsMedicalProcedure, CreateMedicalProcedureDto, UpdateMedicalProcedureDto, MedicalProcedureResponseDto, MedicalProcedureService>,
        IMedicalProcedureService
{
    public MedicalProcedureService(
        IRepository<HmsMedicalProcedure> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateMedicalProcedureDto>? createValidator,
        IValidator<UpdateMedicalProcedureDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<MedicalProcedureService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<MedicalProcedureResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? visitId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            visitId is null ? null : e => e.VisitId == visitId.Value,
            cancellationToken);
}

public sealed class PrescriptionService
    : HmsCrudServiceBase<HmsPrescription, CreatePrescriptionDto, UpdatePrescriptionDto, PrescriptionResponseDto, PrescriptionService>,
        IPrescriptionService
{
    public PrescriptionService(
        IRepository<HmsPrescription> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePrescriptionDto>? createValidator,
        IValidator<UpdatePrescriptionDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PrescriptionService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override async Task OnBeforeCreateAsync(
        HmsPrescription entity,
        CreatePrescriptionDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity.PrescriptionNo))
            entity.PrescriptionNo = HmsDocumentNumberHelper.Generate("PRX");
        await Task.CompletedTask;
    }

    public Task<BaseResponse<PagedResponse<PrescriptionResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? visitId,
        long? patientId,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<HmsPrescription, bool>>? pred = null;
        if (visitId.HasValue || patientId.HasValue)
        {
            var v = visitId;
            var p = patientId;
            pred = e =>
                (!v.HasValue || e.VisitId == v.Value) &&
                (!p.HasValue || e.PatientId == p.Value);
        }

        return GetPagedCoreAsync(query, pred, cancellationToken);
    }
}

public sealed class PrescriptionItemService
    : HmsCrudServiceBase<HmsPrescriptionItem, CreatePrescriptionItemDto, UpdatePrescriptionItemDto, PrescriptionItemResponseDto, PrescriptionItemService>,
        IPrescriptionItemService
{
    public PrescriptionItemService(
        IRepository<HmsPrescriptionItem> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePrescriptionItemDto>? createValidator,
        IValidator<UpdatePrescriptionItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PrescriptionItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PrescriptionItemResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? prescriptionId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            prescriptionId is null ? null : e => e.PrescriptionId == prescriptionId.Value,
            cancellationToken);
}

public sealed class PrescriptionNoteService
    : HmsCrudServiceBase<HmsPrescriptionNote, CreatePrescriptionNoteDto, UpdatePrescriptionNoteDto, PrescriptionNoteResponseDto, PrescriptionNoteService>,
        IPrescriptionNoteService
{
    public PrescriptionNoteService(
        IRepository<HmsPrescriptionNote> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePrescriptionNoteDto>? createValidator,
        IValidator<UpdatePrescriptionNoteDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PrescriptionNoteService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PrescriptionNoteResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? prescriptionId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            prescriptionId is null ? null : e => e.PrescriptionId == prescriptionId.Value,
            cancellationToken);
}

public sealed class PaymentModeService
    : HmsCrudServiceBase<HmsPaymentMode, CreatePaymentModeDto, UpdatePaymentModeDto, PaymentModeResponseDto, PaymentModeService>,
        IPaymentModeService
{
    public PaymentModeService(
        IRepository<HmsPaymentMode> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePaymentModeDto>? createValidator,
        IValidator<UpdatePaymentModeDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PaymentModeService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<PaymentModeResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public sealed class BillingHeaderService
    : HmsCrudServiceBase<HmsBillingHeader, CreateBillingHeaderDto, UpdateBillingHeaderDto, BillingHeaderResponseDto, BillingHeaderService>,
        IBillingHeaderService
{
    public BillingHeaderService(
        IRepository<HmsBillingHeader> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateBillingHeaderDto>? createValidator,
        IValidator<UpdateBillingHeaderDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<BillingHeaderService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override async Task OnBeforeCreateAsync(
        HmsBillingHeader entity,
        CreateBillingHeaderDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity.BillNo))
            entity.BillNo = HmsDocumentNumberHelper.Generate("BILL");
        await Task.CompletedTask;
    }

    public Task<BaseResponse<PagedResponse<BillingHeaderResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? visitId,
        long? patientId,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<HmsBillingHeader, bool>>? pred = null;
        if (visitId.HasValue || patientId.HasValue)
        {
            var v = visitId;
            var p = patientId;
            pred = e =>
                (!v.HasValue || e.VisitId == v.Value) &&
                (!p.HasValue || e.PatientId == p.Value);
        }

        return GetPagedCoreAsync(query, pred, cancellationToken);
    }
}

public sealed class BillingItemService
    : HmsCrudServiceBase<HmsBillingItem, CreateBillingItemDto, UpdateBillingItemDto, BillingItemResponseDto, BillingItemService>,
        IBillingItemService
{
    public BillingItemService(
        IRepository<HmsBillingItem> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateBillingItemDto>? createValidator,
        IValidator<UpdateBillingItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<BillingItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<BillingItemResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? billingHeaderId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            billingHeaderId is null ? null : e => e.BillingHeaderId == billingHeaderId.Value,
            cancellationToken);
}

public sealed class PaymentTransactionService
    : HmsCrudServiceBase<HmsPaymentTransaction, CreatePaymentTransactionDto, UpdatePaymentTransactionDto, PaymentTransactionResponseDto, PaymentTransactionService>,
        IPaymentTransactionService
{
    public PaymentTransactionService(
        IRepository<HmsPaymentTransaction> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePaymentTransactionDto>? createValidator,
        IValidator<UpdatePaymentTransactionDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PaymentTransactionService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PaymentTransactionResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? billingHeaderId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            billingHeaderId is null ? null : e => e.BillingHeaderId == billingHeaderId.Value,
            cancellationToken);
}

public sealed class VisitTypeService
    : HmsCrudServiceBase<HmsVisitType, CreateVisitTypeDto, UpdateVisitTypeDto, VisitTypeResponseDto, VisitTypeService>,
        IVisitTypeService
{
    public VisitTypeService(
        IRepository<HmsVisitType> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateVisitTypeDto>? createValidator,
        IValidator<UpdateVisitTypeDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<VisitTypeService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<VisitTypeResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}
