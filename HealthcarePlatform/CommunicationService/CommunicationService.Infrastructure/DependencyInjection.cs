using CommunicationService.Application.Options;
using CommunicationService.Application.Abstractions;
using CommunicationService.Domain.Repositories;
using CommunicationService.Infrastructure.Background;
using CommunicationService.Infrastructure.Persistence;
using CommunicationService.Infrastructure.Persistence.Repositories;
using CommunicationService.Infrastructure.Services;
using Healthcare.Common.Events;
using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CommunicationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCommunicationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddSingleton<IEventPublisher, NoOpEventPublisher>();

        services.Configure<CommunicationOptions>(configuration.GetSection(CommunicationOptions.SectionName));

        services.AddDbContext<CommunicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();
        services.AddScoped<INotificationQueueRepository, NotificationQueueRepository>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IWhatsAppService, WhatsAppService>();

        services.AddHttpClient("CommunicationSms", (sp, c) =>
        {
            var o = sp.GetRequiredService<IOptions<CommunicationOptions>>().Value;
            var baseUrl = o.Sms.BaseUrl.TrimEnd('/') + "/";
            c.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient("CommunicationWhatsApp", (sp, c) =>
        {
            var o = sp.GetRequiredService<IOptions<CommunicationOptions>>().Value;
            var baseUrl = o.WhatsApp.BaseUrl.TrimEnd('/') + "/";
            c.BaseAddress = new Uri(baseUrl);
        });

        services.AddHostedService<NotificationProcessor>();

        return services;
    }
}
