using CommunicationService.Contracts.Notifications;
using Healthcare.Common.Events;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using LISService.Application.Options;
using LISService.Domain.Repositories;
using LISService.Infrastructure.Notifications;
using LISService.Infrastructure.Persistence;
using LISService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LISService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddLisInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddSingleton<IEventPublisher, NoOpEventPublisher>();
        services.AddSharedEnterpriseIntegration(configuration);

        services.Configure<LisNotificationIntegrationOptions>(
            configuration.GetSection(LisNotificationIntegrationOptions.SectionName));

        services.AddHttpClient<INotificationApiClient, NotificationApiClient>((sp, client) =>
        {
            var baseUrl = configuration["Communication:BaseUrl"] ?? "http://localhost:5800/";
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        });

        services.AddDbContext<LisDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("LisDatabase")));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return services;
    }
}
