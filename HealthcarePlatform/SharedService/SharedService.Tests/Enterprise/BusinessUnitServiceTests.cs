using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Infrastructure.Services.Enterprise;
using Xunit;

namespace SharedService.Tests.Enterprise;

public sealed class BusinessUnitServiceTests
{
    [Fact]
    public async Task GetByIdAsync_Should_Fail_When_Missing()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new BusinessUnitService(db, tenant.Object, NullLogger<BusinessUnitService>.Instance);

        var result = await sut.GetByIdAsync(999);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_When_Company_Missing()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new BusinessUnitService(db, tenant.Object, NullLogger<BusinessUnitService>.Instance);

        var result = await sut.CreateAsync(new CreateBusinessUnitDto
        {
            CompanyId = 404,
            BusinessUnitCode = "B",
            BusinessUnitName = "B",
            BusinessUnitType = "Hospital"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Company");
    }

    [Fact]
    public async Task CreateAsync_Should_Succeed_When_Company_Exists()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var (_, companyId, _, _) = SharedEnterpriseTestData.SeedHierarchy(db);

        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new BusinessUnitService(db, tenant.Object, NullLogger<BusinessUnitService>.Instance);

        var result = await sut.CreateAsync(new CreateBusinessUnitDto
        {
            CompanyId = companyId,
            BusinessUnitCode = " BU2 ",
            BusinessUnitName = " Unit2 ",
            BusinessUnitType = " Lab "
        });

        result.Success.Should().BeTrue();
        result.Data!.BusinessUnitCode.Should().Be("BU2");
    }

    [Fact]
    public async Task ListAsync_Should_Filter_By_CompanyId()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var (_, companyId, businessUnitId, _) = SharedEnterpriseTestData.SeedHierarchy(db);

        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new BusinessUnitService(db, tenant.Object, NullLogger<BusinessUnitService>.Instance);

        var filtered = await sut.ListAsync(companyId);
        filtered.Data.Should().ContainSingle(b => b.Id == businessUnitId);
    }
}
