using System.Reflection;
using FluentValidation;
using HMSService.Application.Mapping;
using HMSService.Application.Services;
using HMSService.Application.Services.Extended;
using HMSService.Application.Services.Gap;
using HMSService.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HMSService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddHmsApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(HmsMappingProfile));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IVisitService, VisitService>();
        services.AddScoped<INotificationHelper, NotificationHelper>();

        services.AddScoped<IAppointmentStatusHistoryService, AppointmentStatusHistoryService>();
        services.AddScoped<IAppointmentQueueService, AppointmentQueueService>();
        services.AddScoped<IVitalService, VitalService>();
        services.AddScoped<IClinicalNoteService, ClinicalNoteService>();
        services.AddScoped<IDiagnosisService, DiagnosisService>();
        services.AddScoped<IMedicalProcedureService, MedicalProcedureService>();
        services.AddScoped<IPrescriptionService, PrescriptionService>();
        services.AddScoped<IPrescriptionItemService, PrescriptionItemService>();
        services.AddScoped<IPrescriptionNoteService, PrescriptionNoteService>();
        services.AddScoped<IPaymentModeService, PaymentModeService>();
        services.AddScoped<IBillingHeaderService, BillingHeaderService>();
        services.AddScoped<IBillingItemService, BillingItemService>();
        services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
        services.AddScoped<IVisitTypeService, VisitTypeService>();

        services.AddScoped<IPatientMasterService, PatientMasterService>();
        services.AddScoped<IAdmissionWorkflowService, AdmissionWorkflowService>();
        services.AddScoped<IWardService, WardService>();
        services.AddScoped<IBedService, BedService>();
        services.AddScoped<IHousekeepingStatusService, HousekeepingStatusService>();
        services.AddScoped<IEmarEntryService, EmarEntryService>();
        services.AddScoped<IDoctorOrderAlertService, DoctorOrderAlertService>();

        services.AddScoped<IOperationTheatreService, OperationTheatreService>();
        services.AddScoped<ISurgeryScheduleService, SurgeryScheduleService>();
        services.AddScoped<IAnesthesiaRecordService, AnesthesiaRecordService>();
        services.AddScoped<IPostOpRecordService, PostOpRecordService>();
        services.AddScoped<IOtConsumableService, OtConsumableService>();
        services.AddScoped<IPricingRuleService, PricingRuleService>();
        services.AddScoped<IPackageDefinitionService, PackageDefinitionService>();
        services.AddScoped<IPackageDefinitionLineService, PackageDefinitionLineService>();
        services.AddScoped<IProformaInvoiceService, ProformaInvoiceService>();
        services.AddScoped<IInsuranceProviderService, InsuranceProviderService>();
        services.AddScoped<IPreAuthorizationService, PreAuthorizationService>();
        services.AddScoped<IClaimService, ClaimService>();

        return services;
    }
}
