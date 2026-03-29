using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PharmacyService.Application.Services;
using Xunit;

namespace PharmacyService.Tests.Services;

public sealed class LmsInventoryIntegrationServiceTests
{
    [Fact]
    public async Task RecordLmsReagentConsumptionAsync_ReturnsSuccess()
    {
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1);
        tenant.SetupGet(t => t.FacilityId).Returns(2L);

        var sut = new LmsInventoryIntegrationService(tenant.Object, NullLogger<LmsInventoryIntegrationService>.Instance);
        var result = await sut.RecordLmsReagentConsumptionAsync(
            new RecordLmsReagentConsumptionRequest(1, null, 2.5m, "LMS-TEST", null));

        result.Success.Should().BeTrue();
    }
}
