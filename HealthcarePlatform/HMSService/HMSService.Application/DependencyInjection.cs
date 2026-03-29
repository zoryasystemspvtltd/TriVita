using System.Reflection;
using FluentValidation;
using HMSService.Application.Mapping;
using HMSService.Application.Services;
using HMSService.Application.Services.Extended;
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

        return services;
    }
}
