using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Healthcare.Common.Integration.SharedService;

public static class SharedEnterpriseIntegrationExtensions
{
    /// <summary>Registers <see cref="IFacilityTenantValidator"/> (typed HttpClient to SharedService).</summary>
    public static IServiceCollection AddSharedEnterpriseIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SharedServiceClientOptions>(configuration.GetSection(SharedServiceClientOptions.SectionName));
        services.AddHttpClient<IFacilityTenantValidator, SharedEnterpriseApiClient>((sp, client) =>
        {
            var opt = sp.GetRequiredService<IOptions<SharedServiceClientOptions>>().Value;
            var baseUrl = string.IsNullOrWhiteSpace(opt.BaseUrl) ? "http://localhost:5153/" : opt.BaseUrl;
            client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            var seconds = Math.Clamp(opt.TimeoutSeconds, 5, 120);
            client.Timeout = TimeSpan.FromSeconds(seconds);
        });

        return services;
    }
}
