using CommunicationService.Contracts.Notifications;
using Healthcare.Common.Events;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyService.Application.Options;
using PharmacyService.Domain.Repositories;
using PharmacyService.Infrastructure.Notifications;
using PharmacyService.Infrastructure.Persistence;
using PharmacyService.Infrastructure.Persistence.Repositories;

namespace PharmacyService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPharmacyInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddSingleton<IEventPublisher, NoOpEventPublisher>();
        services.AddSharedEnterpriseIntegration(configuration);

        services.Configure<PharmacyNotificationIntegrationOptions>(
            configuration.GetSection(PharmacyNotificationIntegrationOptions.SectionName));

        services.AddHttpClient<INotificationApiClient, NotificationApiClient>((sp, client) =>
        {
            var baseUrl = configuration["Communication:BaseUrl"] ?? "http://localhost:5800/";
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
        });

        services.AddDbContext<PharmacyDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PharmacyDatabase")));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return services;
    }
}
