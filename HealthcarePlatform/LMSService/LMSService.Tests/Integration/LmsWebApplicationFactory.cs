using LMSService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LMSService.Tests.Integration;

public sealed class LmsWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("IntegrationTest:UseTestAuth", "true");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LmsDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<LmsDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("LmsIntegrationTests");
            });
        });
    }
}
