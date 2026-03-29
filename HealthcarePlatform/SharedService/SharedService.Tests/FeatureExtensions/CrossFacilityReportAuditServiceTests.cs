using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Application.Validation;
using SharedService.Infrastructure.Persistence;
using SharedService.Infrastructure.Services.FeatureExtensions;
using Xunit;

namespace SharedService.Tests.FeatureExtensions;

public sealed class CrossFacilityReportAuditServiceTests
{
    [Fact]
    public async Task CreateAndGetPaged_Should_Work()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        await sut.CreateAsync(new CreateCrossFacilityReportAuditDto { ReportCode = "RPT1", ReportName = "N1" });
        await sut.CreateAsync(new CreateCrossFacilityReportAuditDto { ReportCode = "RPT2" });

        var paged = await sut.GetPagedAsync(new PagedQuery { Page = 1, PageSize = 10 }, null);

        paged.Success.Should().BeTrue();
        paged.Data!.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_Should_Set_CompletedOn()
    {
        await using var db = FeatureExtension09TestSeed.CreateInMemoryContext();
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);
        var created = await sut.CreateAsync(new CreateCrossFacilityReportAuditDto { ReportCode = "X" });
        var completed = DateTime.UtcNow;

        var updated = await sut.UpdateAsync(created.Data!.Id, new UpdateCrossFacilityReportAuditDto
        {
            ResultRowCount = 42,
            CompletedOn = completed
        });

        updated.Success.Should().BeTrue();
        updated.Data!.ResultRowCount.Should().Be(42);
    }

    private static CrossFacilityReportAuditService CreateSut(SharedDbContext db, ITenantContext tenant) =>
        new(
            db,
            tenant,
            new CreateCrossFacilityReportAuditDtoValidator(),
            new UpdateCrossFacilityReportAuditDtoValidator(),
            NullLogger<CrossFacilityReportAuditService>.Instance);

    private static Mock<ITenantContext> MockTenant(long tenantId, long userId)
    {
        var m = new Mock<ITenantContext>();
        m.SetupGet(t => t.TenantId).Returns(tenantId);
        m.SetupGet(t => t.UserId).Returns(userId);
        return m;
    }
}
