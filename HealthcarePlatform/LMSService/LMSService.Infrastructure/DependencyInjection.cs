using CommunicationService.Contracts.Notifications;
using Healthcare.Common.Events;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using LMSService.Application.Options;
using LMSService.Domain.Repositories;
using LMSService.Infrastructure.Notifications;
using LMSService.Infrastructure.Persistence;
using LMSService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMSService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLmsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddSingleton<IEventPublisher, NoOpEventPublisher>();
        services.AddSharedEnterpriseIntegration(configuration);

        services.Configure<LmsNotificationIntegrationOptions>(
            configuration.GetSection(LmsNotificationIntegrationOptions.SectionName));

        services.AddHttpClient<INotificationApiClient, NotificationApiClient>((sp, client) =>
        {
            var baseUrl = configuration["Communication:BaseUrl"] ?? "http://localhost:5800/";
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        });

        services.AddDbContext<LmsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return services;
    }
}
