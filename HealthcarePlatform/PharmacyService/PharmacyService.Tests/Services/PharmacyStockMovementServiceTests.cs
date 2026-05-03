using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Moq;
using PharmacyService.Application.Abstractions;
using PharmacyService.Application.Services.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;
using Xunit;

namespace PharmacyService.Tests.Services;

public sealed class PharmacyStockMovementServiceTests
{
    private readonly Mock<IRepository<PhrStockLedger>> _ledger = new();
    private readonly Mock<IRepository<PhrBatchStock>> _batchStock = new();
    private readonly Mock<IRepository<PhrMedicineBatch>> _batches = new();
    private readonly Mock<IPharmacyLockedStockReader> _locked = new();
    private readonly Mock<ITenantContext> _tenant = new();

    public PharmacyStockMovementServiceTests()
    {
        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(1);
        _tenant.SetupGet(t => t.FacilityId).Returns(100L);
    }

    private PharmacyStockMovementService CreateSut() =>
        new(_ledger.Object, _batchStock.Object, _batches.Object, _locked.Object, _tenant.Object);

    [Fact]
    public async Task ApplyMovementAsync_FailsWhenDeltaZero()
    {
        var sut = CreateSut();
        var r = await sut.ApplyMovementAsync(
            StockLedgerTransactionType.GRN,
            1,
            null,
            10,
            20,
            0,
            DateTime.UtcNow,
            null,
            null);

        r.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyMovementAsync_FailsWhenFacilityMissing()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);
        var sut = CreateSut();
        var r = await sut.ApplyMovementAsync(
            StockLedgerTransactionType.GRN,
            1,
            null,
            10,
            20,
            5,
            DateTime.UtcNow,
            null,
            null);

        r.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyMovementAsync_FailsWhenBatchMissing()
    {
        _batches.Setup(x => x.GetByIdAsync(20, It.IsAny<CancellationToken>())).ReturnsAsync((PhrMedicineBatch?)null);
        var sut = CreateSut();
        var r = await sut.ApplyMovementAsync(
            StockLedgerTransactionType.GRN,
            1,
            null,
            10,
            20,
            5,
            DateTime.UtcNow,
            null,
            null);

        r.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyMovementAsync_Deduction_FailsWhenMedicineAggregateInsufficient()
    {
        var batch = new PhrMedicineBatch { Id = 20, MedicineId = 10, TenantId = 1, IsDeleted = false };
        _batches.Setup(x => x.GetByIdAsync(20, It.IsAny<CancellationToken>())).ReturnsAsync(batch);
        _locked.Setup(x => x.SumMedicineFacilityStockLockedAsync(10, 100, It.IsAny<CancellationToken>())).ReturnsAsync(2m);
        var sut = CreateSut();
        var r = await sut.ApplyMovementAsync(
            StockLedgerTransactionType.SALE,
            1,
            1,
            10,
            20,
            -5m,
            DateTime.UtcNow,
            null,
            null);

        r.Success.Should().BeFalse();
        r.Message.Should().Contain("Insufficient stock");
        _locked.Verify(x => x.GetBatchStockRowLockedAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApplyMovementAsync_Deduction_FailsWhenBatchInsufficient()
    {
        var batch = new PhrMedicineBatch { Id = 20, MedicineId = 10, TenantId = 1, IsDeleted = false };
        _batches.Setup(x => x.GetByIdAsync(20, It.IsAny<CancellationToken>())).ReturnsAsync(batch);
        _locked.Setup(x => x.SumMedicineFacilityStockLockedAsync(10, 100, It.IsAny<CancellationToken>())).ReturnsAsync(100m);
        _locked.Setup(x => x.GetBatchStockRowLockedAsync(20, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PhrBatchStock { CurrentQty = 2m, MedicineBatchId = 20, FacilityId = 100, TenantId = 1 });
        var sut = CreateSut();
        var r = await sut.ApplyMovementAsync(
            StockLedgerTransactionType.SALE,
            1,
            1,
            10,
            20,
            -5m,
            DateTime.UtcNow,
            null,
            null);

        r.Success.Should().BeFalse();
        r.Message.Should().Contain("Insufficient stock");
    }

    [Fact]
    public async Task AllocateSaleFefoAsync_FailsWhenQuantityNotPositive()
    {
        var sut = CreateSut();
        var r = await sut.AllocateSaleFefoAsync(10, 0);
        r.Success.Should().BeFalse();
    }

    [Fact]
    public async Task AllocateSaleFefoAsync_FailsWhenMedicineTotalInsufficient()
    {
        _locked.Setup(x => x.SumMedicineFacilityStockLockedAsync(10, 100, It.IsAny<CancellationToken>())).ReturnsAsync(1m);
        var sut = CreateSut();
        var r = await sut.AllocateSaleFefoAsync(10, 5m);
        r.Success.Should().BeFalse();
        r.Message.Should().Contain("Insufficient stock");
    }
}
