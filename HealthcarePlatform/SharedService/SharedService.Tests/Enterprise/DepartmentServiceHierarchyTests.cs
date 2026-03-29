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

public sealed class DepartmentServiceHierarchyTests
{
    [Fact]
    public async Task UpdateAsync_WhenParentWouldCreateCycle_ReturnsFailure()
    {
        await using var db = CreateInMemoryContext();
        SeedMinimalHierarchy(db);

        var parent = db.Departments.Single(d => d.DepartmentCode == "P");
        var child = db.Departments.Single(d => d.DepartmentCode == "C");

        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1L);
        tenant.SetupGet(t => t.UserId).Returns(1L);

        var sut = new DepartmentService(db, tenant.Object, NullLogger<DepartmentService>.Instance);

        var dto = new UpdateDepartmentDto
        {
            FacilityParentId = parent.FacilityParentId,
            DepartmentCode = parent.DepartmentCode,
            DepartmentName = parent.DepartmentName,
            DepartmentType = parent.DepartmentType,
            ParentDepartmentId = child.Id,
            IsActive = true
        };

        var result = await sut.UpdateAsync(parent.Id, dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("circular");
    }

    private static SharedDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<SharedDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SharedDbContext(options);
    }

    private static void SeedMinimalHierarchy(SharedDbContext db)
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

        var parentDept = new Department
        {
            TenantId = 1,
            FacilityId = fac.Id,
            FacilityParentId = fac.Id,
            DepartmentCode = "P",
            DepartmentName = "Parent",
            DepartmentType = "OPD",
            ParentDepartmentId = null,
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Departments.Add(parentDept);
        db.SaveChanges();

        var childDept = new Department
        {
            TenantId = 1,
            FacilityId = fac.Id,
            FacilityParentId = fac.Id,
            DepartmentCode = "C",
            DepartmentName = "Child",
            DepartmentType = "OPD",
            ParentDepartmentId = parentDept.Id,
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Departments.Add(childDept);
        db.SaveChanges();
    }
}
