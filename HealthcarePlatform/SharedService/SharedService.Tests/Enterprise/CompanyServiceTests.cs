using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Infrastructure.Services.Enterprise;
using Xunit;

namespace SharedService.Tests.Enterprise;

public sealed class CompanyServiceTests
{
    [Fact]
    public async Task GetByIdAsync_Should_Fail_When_Missing()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new CompanyService(db, tenant.Object, NullLogger<CompanyService>.Instance);

        var result = await sut.GetByIdAsync(999);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_When_Enterprise_Missing()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new CompanyService(db, tenant.Object, NullLogger<CompanyService>.Instance);

        var result = await sut.CreateAsync(new CreateCompanyDto
        {
            EnterpriseId = 404,
            CompanyCode = "X",
            CompanyName = "X"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Enterprise");
    }

    [Fact]
    public async Task CreateAsync_Should_Succeed_When_Enterprise_Exists()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var (enterpriseId, _, _, _) = SharedEnterpriseTestData.SeedHierarchy(db);

        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new CompanyService(db, tenant.Object, NullLogger<CompanyService>.Instance);

        var result = await sut.CreateAsync(new CreateCompanyDto
        {
            EnterpriseId = enterpriseId,
            CompanyCode = " C2 ",
            CompanyName = " Second "
        });

        result.Success.Should().BeTrue();
        result.Data!.CompanyCode.Should().Be("C2");
    }

    [Fact]
    public async Task ListAsync_Should_Filter_By_EnterpriseId()
    {
        await using var db = SharedEnterpriseTestData.CreateInMemoryContext();
        var (enterpriseId, companyId, _, _) = SharedEnterpriseTestData.SeedHierarchy(db);

        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new CompanyService(db, tenant.Object, NullLogger<CompanyService>.Instance);

        var all = await sut.ListAsync(null);
        all.Data.Should().HaveCount(1);

        var filtered = await sut.ListAsync(enterpriseId);
        filtered.Data.Should().ContainSingle(c => c.Id == companyId);
    }
}
