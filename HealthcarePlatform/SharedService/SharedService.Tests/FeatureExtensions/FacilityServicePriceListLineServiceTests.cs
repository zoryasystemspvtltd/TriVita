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

public sealed class FacilityServicePriceListLineServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Create_When_PriceList_Exists()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var facId = await FeatureExtension09TestSeed.SeedFacilityAsync(db);
        var tenant = MockTenant(1, 1);
        var listSvc = new FacilityServicePriceListService(
            db,
            tenant.Object,
            new CreateFacilityServicePriceListDtoValidator(),
            new UpdateFacilityServicePriceListDtoValidator(),
            NullLogger<FacilityServicePriceListService>.Instance);
        var created = await listSvc.CreateAsync(new CreateFacilityServicePriceListDto
        {
            FacilityId = facId,
            PriceListCode = "PL",
            PriceListName = "PL",
            ServiceModule = "HMS"
        });
        var listId = created.Data!.Id;

        var lineSvc = CreateLineSut(db, tenant.Object);
        var line = await lineSvc.CreateAsync(new CreateFacilityServicePriceListLineDto
        {
            FacilityId = facId,
            PriceListId = listId,
            ServiceItemCode = "SVC1",
            UnitPrice = 10.5m
        });

        line.Success.Should().BeTrue();
        line.Data!.UnitPrice.Should().Be(10.5m);
    }

    [Fact]
    public async Task ListByPriceListAsync_Should_Fail_When_Header_Missing()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var facId = await FeatureExtension09TestSeed.SeedFacilityAsync(db);
        var tenant = MockTenant(1, 1);
        var lineSvc = CreateLineSut(db, tenant.Object);

        var result = await lineSvc.ListByPriceListAsync(facId, 999);

        result.Success.Should().BeFalse();
    }

    private static FacilityServicePriceListLineService CreateLineSut(SharedDbContext db, ITenantContext tenant) =>
        new(
            db,
            tenant,
            new CreateFacilityServicePriceListLineDtoValidator(),
            new UpdateFacilityServicePriceListLineDtoValidator(),
            NullLogger<FacilityServicePriceListLineService>.Instance);

    private static Mock<ITenantContext> MockTenant(long tenantId, long userId)
    {
        var m = new Mock<ITenantContext>();
        m.SetupGet(t => t.TenantId).Returns(tenantId);
        m.SetupGet(t => t.UserId).Returns(userId);
        return m;
    }
}
