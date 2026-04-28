using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Validation;
using SharedService.Domain.Enterprise;
using SharedService.Infrastructure.Persistence;
using SharedService.Infrastructure.Services.Enterprise;
using Xunit;

namespace SharedService.Tests.Enterprise;

public sealed class EnterpriseB2BContractServiceTests
{
    [Fact]
    public async Task ListByEnterpriseAsync_Should_Fail_When_Enterprise_Missing()
    {
        await using var db = SharedEnterpriseTestData.CreateContext();
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var result = await sut.ListByEnterpriseAsync(99);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ListByEnterpriseAsync_Should_Return_Empty_When_None()
    {
        await using var db = SharedEnterpriseTestData.CreateContext();
        var entId = await SeedEnterprise(db, 1);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var result = await sut.ListByEnterpriseAsync(entId);

        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_When_Valid()
    {
        await using var db = SharedEnterpriseTestData.CreateContext();
        var entId = await SeedEnterprise(db, 1);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var result = await sut.CreateAsync(new CreateEnterpriseB2BContractDto
        {
            EnterpriseId = entId,
            PartnerType = "Insurance",
            PartnerName = "Acme Ins",
            ContractCode = "CNT-1"
        });

        result.Success.Should().BeTrue();
        result.Data!.ContractCode.Should().Be("CNT-1");
        (await db.EnterpriseB2BContracts.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_On_Duplicate_Code()
    {
        await using var db = SharedEnterpriseTestData.CreateContext();
        var entId = await SeedEnterprise(db, 1);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        await sut.CreateAsync(new CreateEnterpriseB2BContractDto
        {
            EnterpriseId = entId,
            PartnerType = "Corporate",
            PartnerName = "Corp",
            ContractCode = "SAME"
        });

        var second = await sut.CreateAsync(new CreateEnterpriseB2BContractDto
        {
            EnterpriseId = entId,
            PartnerType = "Corporate",
            PartnerName = "Corp2",
            ContractCode = "SAME"
        });

        second.Success.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_When_Found()
    {
        await using var db = SharedEnterpriseTestData.CreateContext();
        var entId = await SeedEnterprise(db, 1);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var created = await sut.CreateAsync(new CreateEnterpriseB2BContractDto
        {
            EnterpriseId = entId,
            PartnerType = "Corporate",
            PartnerName = "A",
            ContractCode = "C1"
        });

        var updated = await sut.UpdateAsync(created.Data!.Id, new UpdateEnterpriseB2BContractDto
        {
            PartnerType = "Corporate",
            PartnerName = "B",
            ContractCode = "C1",
            IsActive = true
        });

        updated.Success.Should().BeTrue();
        updated.Data!.PartnerName.Should().Be("B");
    }

    [Fact]
    public async Task DeleteAsync_Should_SoftDelete()
    {
        await using var db = SharedEnterpriseTestData.CreateContext();
        var entId = await SeedEnterprise(db, 1);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var created = await sut.CreateAsync(new CreateEnterpriseB2BContractDto
        {
            EnterpriseId = entId,
            PartnerType = "Corporate",
            PartnerName = "A",
            ContractCode = "DEL"
        });

        var del = await sut.DeleteAsync(created.Data!.Id);
        del.Success.Should().BeTrue();

        var row = await db.EnterpriseB2BContracts.SingleAsync();
        row.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_Validation_When_EffectiveRange_Invalid()
    {
        await using var db = SharedEnterpriseTestData.CreateContext();
        var entId = await SeedEnterprise(db, 1);
        var tenant = MockTenant(1, 1);
        var sut = CreateSut(db, tenant.Object);

        var result = await sut.CreateAsync(new CreateEnterpriseB2BContractDto
        {
            EnterpriseId = entId,
            PartnerType = "Corporate",
            PartnerName = "A",
            ContractCode = "X",
            EffectiveFrom = DateTime.UtcNow.Date,
            EffectiveTo = DateTime.UtcNow.Date.AddDays(-1)
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("EffectiveTo");
    }

    private static EnterpriseB2BContractService CreateSut(SharedDbContext db, ITenantContext tenant) =>
        new(
            db,
            tenant,
            new CreateEnterpriseB2BContractDtoValidator(),
            new UpdateEnterpriseB2BContractDtoValidator(),
            NullLogger<EnterpriseB2BContractService>.Instance);

    private static Mock<ITenantContext> MockTenant(long tenantId, long userId)
    {
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(tenantId);
        tenant.SetupGet(t => t.UserId).Returns(userId);
        return tenant;
    }

    private static async Task<long> SeedEnterprise(SharedDbContext db, long tenantId)
    {
        var now = DateTime.UtcNow;
        var ent = new EnterpriseRoot
        {
            TenantId = tenantId,
            EnterpriseCode = "E-B2B",
            EnterpriseName = "E",
            IsActive = true,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = 1,
            ModifiedBy = 1
        };
        db.Enterprises.Add(ent);
        await db.SaveChangesAsync();
        return ent.Id;
    }

}
