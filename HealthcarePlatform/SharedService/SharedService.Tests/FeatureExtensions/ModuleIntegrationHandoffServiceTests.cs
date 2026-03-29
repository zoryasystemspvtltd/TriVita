using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Application.Validation;
using SharedService.Infrastructure.Persistence;
using SharedService.Infrastructure.Services.FeatureExtensions;
using Xunit;

namespace SharedService.Tests.FeatureExtensions;

public sealed class ModuleIntegrationHandoffServiceTests
{
    [Fact]
    public async Task CreateAndListByCorrelation_Should_Work()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        await sut.CreateAsync(new CreateModuleIntegrationHandoffDto
        {
            CorrelationId = "corr-1",
            SourceModule = "HMS",
            TargetModule = "LIS",
            EntityType = "LabOrder",
            StatusCode = "Pending"
        });

        var list = await sut.ListByCorrelationAsync("corr-1");

        list.Success.Should().BeTrue();
        list.Data.Should().HaveCount(1);
        list.Data![0].SourceModule.Should().Be("HMS");
    }

    [Fact]
    public async Task ListByCorrelation_Should_Fail_When_Empty()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var result = await sut.ListByCorrelationAsync("  ");

        result.Success.Should().BeFalse();
    }

    private static ModuleIntegrationHandoffService CreateSut(SharedDbContext db, ITenantContext tenant) =>
        new(
            db,
            tenant,
            new CreateModuleIntegrationHandoffDtoValidator(),
            new UpdateModuleIntegrationHandoffDtoValidator(),
            NullLogger<ModuleIntegrationHandoffService>.Instance);

    private static Mock<ITenantContext> MockTenant(long tenantId, long userId)
    {
        var m = new Mock<ITenantContext>();
        m.SetupGet(t => t.TenantId).Returns(tenantId);
        m.SetupGet(t => t.UserId).Returns(userId);
        return m;
    }
}
