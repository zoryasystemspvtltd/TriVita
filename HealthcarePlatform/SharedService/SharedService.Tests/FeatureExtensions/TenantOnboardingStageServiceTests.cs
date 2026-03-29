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

public sealed class TenantOnboardingStageServiceTests
{
    [Fact]
    public async Task Upsert_Should_Create_Then_Update()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var first = await sut.UpsertAsync(new UpsertTenantOnboardingStageDto
        {
            StageCode = "S1",
            StageStatus = "Started"
        });
        first.Success.Should().BeTrue();
        first.Message.Should().Contain("Created");

        var second = await sut.UpsertAsync(new UpsertTenantOnboardingStageDto
        {
            StageCode = "S1",
            StageStatus = "Done",
            CompletedOn = DateTime.UtcNow
        });
        second.Success.Should().BeTrue();
        second.Message.Should().Contain("Updated");
        second.Data!.StageStatus.Should().Be("Done");
    }

    [Fact]
    public async Task GetByStageCode_Should_Find_Row()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);
        await sut.UpsertAsync(new UpsertTenantOnboardingStageDto { StageCode = "X", StageStatus = "A" });

        var got = await sut.GetByStageCodeAsync("X");

        got.Success.Should().BeTrue();
        got.Data!.StageCode.Should().Be("X");
    }

    private static TenantOnboardingStageService CreateSut(SharedDbContext db, ITenantContext tenant) =>
        new(db, tenant, new UpsertTenantOnboardingStageDtoValidator(), NullLogger<TenantOnboardingStageService>.Instance);

    private static Mock<ITenantContext> MockTenant(long tenantId, long userId)
    {
        var m = new Mock<ITenantContext>();
        m.SetupGet(t => t.TenantId).Returns(tenantId);
        m.SetupGet(t => t.UserId).Returns(userId);
        return m;
    }
}
