using LISService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LISService.Tests.Integration;

public sealed class LisWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("IntegrationTest:UseTestAuth", "true");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LisDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<LisDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("LisIntegrationTests");
            });
        });
    }
}
