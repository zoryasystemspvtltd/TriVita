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

public sealed class FacilityServicePriceListServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_When_Facility_Exists()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var facId = await FeatureExtension09TestSeed.SeedFacilityAsync(db);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var result = await sut.CreateAsync(new CreateFacilityServicePriceListDto
        {
            FacilityId = facId,
            PriceListCode = "PL1",
            PriceListName = "List 1",
            ServiceModule = "HMS"
        });

        result.Success.Should().BeTrue();
        result.Data!.PriceListCode.Should().Be("PL1");
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_When_Facility_Missing()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var result = await sut.CreateAsync(new CreateFacilityServicePriceListDto
        {
            FacilityId = 999,
            PriceListCode = "PL1",
            PriceListName = "List 1",
            ServiceModule = "HMS"
        });

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ListByFacilityAsync_Should_Return_Items()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var facId = await FeatureExtension09TestSeed.SeedFacilityAsync(db);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);
        await sut.CreateAsync(new CreateFacilityServicePriceListDto
        {
            FacilityId = facId,
            PriceListCode = "A",
            PriceListName = "A",
            ServiceModule = "LIS"
        });

        var list = await sut.ListByFacilityAsync(facId);

        list.Success.Should().BeTrue();
        list.Data.Should().HaveCount(1);
    }

    private static FacilityServicePriceListService CreateSut(SharedDbContext db, ITenantContext tenant) =>
        new(
            db,
            tenant,
            new CreateFacilityServicePriceListDtoValidator(),
            new UpdateFacilityServicePriceListDtoValidator(),
            NullLogger<FacilityServicePriceListService>.Instance);

    private static Mock<ITenantContext> MockTenant(long tenantId, long userId)
    {
        var m = new Mock<ITenantContext>();
        m.SetupGet(t => t.TenantId).Returns(tenantId);
        m.SetupGet(t => t.UserId).Returns(userId);
        return m;
    }
}
