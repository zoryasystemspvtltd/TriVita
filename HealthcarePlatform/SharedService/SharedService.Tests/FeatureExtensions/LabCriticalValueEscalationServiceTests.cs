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

public sealed class LabCriticalValueEscalationServiceTests
{
    [Fact]
    public async Task CreateAndListByLabResult_Should_Work()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var facId = await FeatureExtension09TestSeed.SeedFacilityAsync(db);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        await sut.CreateAsync(new CreateLabCriticalValueEscalationDto
        {
            FacilityId = facId,
            LabResultId = 500,
            ChannelCode = "SMS",
            EscalationLevel = 1
        });

        var list = await sut.ListByLabResultAsync(facId, 500);

        list.Success.Should().BeTrue();
        list.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdateAsync_Should_Set_AcknowledgedOn()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var facId = await FeatureExtension09TestSeed.SeedFacilityAsync(db);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);
        var created = await sut.CreateAsync(new CreateLabCriticalValueEscalationDto
        {
            FacilityId = facId,
            ChannelCode = "EMAIL"
        });
        var ack = DateTime.UtcNow;

        var updated = await sut.UpdateAsync(created.Data!.Id, new UpdateLabCriticalValueEscalationDto
        {
            AcknowledgedOn = ack,
            OutcomeCode = "OK"
        });

        updated.Success.Should().BeTrue();
        updated.Data!.AcknowledgedOn.Should().BeCloseTo(ack, TimeSpan.FromSeconds(1));
    }

    private static LabCriticalValueEscalationService CreateSut(SharedDbContext db, ITenantContext tenant) =>
        new(
            db,
            tenant,
            new CreateLabCriticalValueEscalationDtoValidator(),
            new UpdateLabCriticalValueEscalationDtoValidator(),
            NullLogger<LabCriticalValueEscalationService>.Instance);

    private static Mock<ITenantContext> MockTenant(long tenantId, long userId)
    {
        var m = new Mock<ITenantContext>();
        m.SetupGet(t => t.TenantId).Returns(tenantId);
        m.SetupGet(t => t.UserId).Returns(userId);
        return m;
    }
}
