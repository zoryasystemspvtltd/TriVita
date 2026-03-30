using CommunicationService.Contracts.Notifications;
using Healthcare.Common.Events;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using HMSService.Application.Integration;
using HMSService.Application.Options;
using HMSService.Infrastructure.Integration;
using HMSService.Domain.Repositories;
using HMSService.Infrastructure.Notifications;
using HMSService.Infrastructure.Persistence;
using HMSService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HMSService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddHmsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddSingleton<IEventPublisher, NoOpEventPublisher>();
        services.AddSharedEnterpriseIntegration(configuration);

        services.Configure<HmsNotificationIntegrationOptions>(
            configuration.GetSection(HmsNotificationIntegrationOptions.SectionName));

        services.AddHttpClient<INotificationApiClient, NotificationApiClient>((sp, client) =>
        {
            var baseUrl = configuration["Communication:BaseUrl"] ?? "http://localhost:5800/";
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        });

        services.Configure<LmsIntegrationOptions>(configuration.GetSection(LmsIntegrationOptions.SectionName));
        services.AddHttpClient<ILmsBillingClient, LmsBillingClient>((sp, client) =>
        {
            var opt = sp.GetRequiredService<IOptions<LmsIntegrationOptions>>().Value;
            client.BaseAddress = new Uri(opt.BaseUrl.TrimEnd('/') + "/");
        });

        services.AddHttpClient<ILmsTestBookingClient, LmsTestBookingClient>((sp, client) =>
        {
            var opt = sp.GetRequiredService<IOptions<LmsIntegrationOptions>>().Value;
            client.BaseAddress = new Uri(opt.BaseUrl.TrimEnd('/') + "/");
        });

        services.AddDbContext<HmsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("HmsDatabase")));

        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IVisitRepository, VisitRepository>();

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return services;
    }
}
