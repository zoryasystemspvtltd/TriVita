using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Infrastructure.Services.Enterprise;
using Xunit;

namespace SharedService.Tests.Enterprise;

public sealed class FacilityServiceTests
{
    [Fact]
    public async Task GetByIdAsync_Should_Fail_When_Missing()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new FacilityService(db, tenant.Object, NullLogger<FacilityService>.Instance);

        var result = await sut.GetByIdAsync(999);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetHierarchyContextAsync_Should_Succeed_For_Seeded_Facility()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var (_, _, _, facilityId) = SharedEnterpriseTestData.SeedHierarchy(db);

        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new FacilityService(db, tenant.Object, NullLogger<FacilityService>.Instance);

        var result = await sut.GetHierarchyContextAsync(facilityId);

        result.Success.Should().BeTrue();
        result.Data!.FacilityCode.Should().Be("F1");
        result.Data.EnterpriseId.Should().BePositive();
    }

    [Fact]
    public async Task GetHierarchyContextAsync_Should_Fail_When_Facility_Missing()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new FacilityService(db, tenant.Object, NullLogger<FacilityService>.Instance);

        var result = await sut.GetHierarchyContextAsync(999);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_When_BusinessUnit_Missing()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new FacilityService(db, tenant.Object, NullLogger<FacilityService>.Instance);

        var result = await sut.CreateAsync(new CreateFacilityDto
        {
            BusinessUnitId = 404,
            FacilityCode = "NF",
            FacilityName = "Nf",
            FacilityType = "Clinic"
        });

        result.Success.Should().BeFalse();
    }
}
