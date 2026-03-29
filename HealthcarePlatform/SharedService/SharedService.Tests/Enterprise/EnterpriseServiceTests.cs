using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;
using SharedService.Infrastructure.Services.Enterprise;
using Xunit;

namespace SharedService.Tests.Enterprise;

public sealed class EnterpriseServiceTests
{
    [Fact]
    public async Task GetByIdAsync_Should_Return_Error_When_NotFound()
    {
        await using var db = CreateContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new EnterpriseService(db, tenant.Object, NullLogger<EnterpriseService>.Instance);

        var result = await sut.GetByIdAsync(999);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task ListAsync_Should_Return_Empty_When_No_Enterprises()
    {
        await using var db = CreateContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new EnterpriseService(db, tenant.Object, NullLogger<EnterpriseService>.Instance);

        var result = await sut.ListAsync();

        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_When_Valid()
    {
        await using var db = CreateContext();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new EnterpriseService(db, tenant.Object, NullLogger<EnterpriseService>.Instance);

        var result = await sut.CreateAsync(new CreateEnterpriseDto
        {
            EnterpriseCode = " ACME ",
            EnterpriseName = " Acme Health "
        });

        result.Success.Should().BeTrue();
        result.Data!.EnterpriseCode.Should().Be("ACME");
        result.Data.EnterpriseName.Should().Be("Acme Health");
    }

    [Fact]
    public async Task GetHierarchyAsync_Should_Return_Tree_When_Enterprise_Exists()
    {
        await using var db = CreateContext();
        SeedHierarchy(db);

        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var entId = db.Enterprises.Single(e => e.EnterpriseCode == "E1").Id;

        var sut = new EnterpriseService(db, tenant.Object, NullLogger<EnterpriseService>.Instance);

        var result = await sut.GetHierarchyAsync(entId);

        result.Success.Should().BeTrue();
        result.Data!.Enterprise.EnterpriseCode.Should().Be("E1");
        result.Data.Companies.Should().HaveCount(1);
        result.Data.Companies[0].BusinessUnits.Should().HaveCount(1);
        result.Data.Companies[0].BusinessUnits[0].Facilities.Should().HaveCount(1);
        result.Data.Companies[0].BusinessUnits[0].Facilities[0].Departments.Should().HaveCount(1);
    }

    private static SharedDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SharedDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SharedDbContext(options);
    }

    private static void SeedHierarchy(SharedDbContext db)
    {
        var now = DateTime.UtcNow;
        var ent = new EnterpriseRoot
        {
            TenantId = 1,
            EnterpriseCode = "E1",
            EnterpriseName = "E1",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Enterprises.Add(ent);
        db.SaveChanges();

        var comp = new Company
        {
            TenantId = 1,
            EnterpriseId = ent.Id,
            CompanyCode = "C1",
            CompanyName = "C1",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Companies.Add(comp);
        db.SaveChanges();

        var bu = new BusinessUnit
        {
            TenantId = 1,
            CompanyId = comp.Id,
            BusinessUnitCode = "BU1",
            BusinessUnitName = "BU1",
            BusinessUnitType = "Hospital",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.BusinessUnits.Add(bu);
        db.SaveChanges();

        var fac = new Facility
        {
            TenantId = 1,
            BusinessUnitId = bu.Id,
            FacilityCode = "F1",
            FacilityName = "F1",
            FacilityType = "Hospital",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Facilities.Add(fac);
        db.SaveChanges();

        db.Departments.Add(new Department
        {
            TenantId = 1,
            FacilityId = fac.Id,
            FacilityParentId = fac.Id,
            DepartmentCode = "D1",
            DepartmentName = "Dept",
            DepartmentType = "OPD",
            ParentDepartmentId = null,
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        });
        db.SaveChanges();
    }
}
