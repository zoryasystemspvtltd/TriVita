using HMSService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HMSService.Tests.Integration;

/// <summary>
/// Hosts HMSService with in-memory EF, test authentication, and <c>IntegrationTest:UseTestAuth=true</c>.
/// </summary>
public sealed class HmsWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("IntegrationTest:UseTestAuth", "true");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<HmsDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<HmsDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("HmsIntegrationTests");
            });
        });
    }
}
