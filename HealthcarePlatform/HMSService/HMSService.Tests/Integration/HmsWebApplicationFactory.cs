using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HMSService.Tests.Integration;

/// <summary>
/// Hosts HMSService with test authentication, and <c>IntegrationTest:UseTestAuth=true</c>.
/// </summary>
public sealed class HmsWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("IntegrationTest:UseTestAuth", "true");
    }
}
