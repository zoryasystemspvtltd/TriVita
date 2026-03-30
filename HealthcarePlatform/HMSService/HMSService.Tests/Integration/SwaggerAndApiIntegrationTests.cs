using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HMSService.Tests.Integration;

/// <summary>
/// Host-level integration tests (Swagger + authenticated API with in-memory DB).
/// </summary>
public sealed class SwaggerAndApiIntegrationTests : IClassFixture<HmsWebApplicationFactory>
{
    private readonly HmsWebApplicationFactory _factory;

    public SwaggerAndApiIntegrationTests(HmsWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SwaggerJson_IsReachable()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VisitTypes_GetPaged_ReturnsOkWithBaseResponse()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/api/v1/visit-types?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("\"success\"");
    }

    [Fact]
    public async Task HmsPricingRules_GetPaged_ReturnsOkWithBaseResponse()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/api/v1/hms/billing/pricing-rules?page=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        json.Should().Contain("\"success\"");
    }
}
