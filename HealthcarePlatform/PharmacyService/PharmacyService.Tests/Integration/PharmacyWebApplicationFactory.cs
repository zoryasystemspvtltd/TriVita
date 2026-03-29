using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PharmacyService.Infrastructure.Persistence;

namespace PharmacyService.Tests.Integration;

public sealed class PharmacyWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("IntegrationTest:UseTestAuth", "true");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PharmacyDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<PharmacyDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("PharmacyIntegrationTests");
            });
        });
    }
}
