using System.Linq;
using AutoMapper;
using FluentAssertions;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Moq;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Mapping;
using PharmacyService.Application.Services.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Xunit;

namespace PharmacyService.Tests.Services;

public sealed class PhrPurchaseBillServiceTests
{
    private readonly Mock<IRepository<PhrPurchaseBill>> _bills = new();
    private readonly Mock<IRepository<PhrPurchaseBillItem>> _items = new();
    private readonly Mock<IRepository<PhrGoodsReceipt>> _grn = new();
    private readonly Mock<IRepository<PhrGoodsReceiptItem>> _grnItems = new();
    private readonly Mock<IRepository<PhrPurchaseOrder>> _pos = new();
    private readonly Mock<IRepository<PhrMedicine>> _meds = new();
    private readonly Mock<IPharmacyUnitOfWork> _uow = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IFacilityTenantValidator> _fac = new();
    private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<PhrGeneratedMappingProfile>()).CreateMapper();
    private readonly Mock<ILogger<PhrPurchaseBillService>> _log = new();

    public PhrPurchaseBillServiceTests()
    {
        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(1);
        _tenant.SetupGet(t => t.FacilityId).Returns(10L);
        _fac.Setup(v => v.GetFacilityContextAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FacilityHierarchyContext
            {
                TenantId = 1,
                FacilityId = 10,
                EnterpriseId = 1,
                CompanyId = 1,
                BusinessUnitId = 1
            });
    }

    private PhrPurchaseBillService Sut() => new(
        _bills.Object,
        _items.Object,
        _grn.Object,
        _grnItems.Object,
        _pos.Object,
        _meds.Object,
        _uow.Object,
        _mapper,
        _tenant.Object,
        _fac.Object,
        createValidator: null,
        updateValidator: null,
        _log.Object);

    [Fact]
    public async Task CreateAsync_Fails_WhenInvoiceDuplicateForSupplier()
    {
        _bills.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PhrPurchaseBill, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PhrPurchaseBill> { new() { Id = 9, InvoiceNo = "INV-1", SupplierId = 5 } });

        _grn.Setup(x => x.GetByIdAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(new PhrGoodsReceipt
        {
            Id = 100,
            TenantId = 1,
            FacilityId = 10,
            SupplierId = 5,
            PurchaseOrderId = 7,
            IsDeleted = false
        });

        _pos.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(new PhrPurchaseOrder
        {
            Id = 7,
            TenantId = 1,
            FacilityId = 10,
            SupplierId = 5,
            IsDeleted = false
        });

        var sut = Sut();
        var r = await sut.CreateAsync(new CreatePurchaseBillDto
        {
            SourceMode = PharmacyPurchaseBillSourceMode.PurchaseOrderLinked,
            PurchaseOrderId = 7,
            GoodsReceiptId = 100,
            SupplierId = 5,
            InvoiceNo = "INV-1",
            InvoiceDate = DateTime.UtcNow.Date,
            DiscountAmount = 0,
            GstPercent = 0,
            OtherTaxAmount = 0
        });

        r.Success.Should().BeFalse();
        r.Message.Should().Contain("unique");
    }

    [Fact]
    public void Service_DoesNotDependOnStockMovement()
    {
        typeof(PhrPurchaseBillService).GetConstructors().Single().GetParameters()
            .Select(p => p.ParameterType)
            .Should().NotContain(t => t.Name.Contains("StockMovement", StringComparison.Ordinal));
    }

    [Fact]
    public async Task CreateAsync_Fails_WhenLineReferencesUnknownGrnItem()
    {
        _bills.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PhrPurchaseBill, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PhrPurchaseBill>());
        _grn.Setup(x => x.GetByIdAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(new PhrGoodsReceipt
        {
            Id = 100,
            TenantId = 1,
            FacilityId = 10,
            SupplierId = 5,
            PurchaseOrderId = null,
            IsDeleted = false
        });
        _grnItems.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PhrGoodsReceiptItem, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PhrGoodsReceiptItem>
            {
                new()
                {
                    Id = 10,
                    GoodsReceiptId = 100,
                    LineNum = 1,
                    QuantityReceived = 5,
                    PurchaseRate = 1m,
                    IsDeleted = false
                }
            });

        var sut = Sut();
        var r = await sut.CreateAsync(new CreatePurchaseBillDto
        {
            SourceMode = PharmacyPurchaseBillSourceMode.DirectGrn,
            PurchaseOrderId = null,
            GoodsReceiptId = 100,
            SupplierId = 5,
            InvoiceNo = "INV-X",
            InvoiceDate = DateTime.UtcNow.Date,
            DiscountAmount = 0,
            GstPercent = 0,
            OtherTaxAmount = 0,
            Items = new List<PurchaseBillLineInputDto>
            {
                new() { GoodsReceiptItemId = 999, Quantity = 1, Rate = 1 }
            }
        });

        r.Success.Should().BeFalse();
        r.Message.Should().Contain("goods receipt");
    }

    [Fact]
    public async Task CreateAsync_Fails_WhenDuplicateGrnItemLines()
    {
        _bills.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PhrPurchaseBill, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PhrPurchaseBill>());
        _grn.Setup(x => x.GetByIdAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(new PhrGoodsReceipt
        {
            Id = 100,
            TenantId = 1,
            FacilityId = 10,
            SupplierId = 5,
            PurchaseOrderId = null,
            IsDeleted = false
        });
        _grnItems.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PhrGoodsReceiptItem, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PhrGoodsReceiptItem>
            {
                new()
                {
                    Id = 10,
                    GoodsReceiptId = 100,
                    LineNum = 1,
                    QuantityReceived = 5,
                    PurchaseRate = 1m,
                    IsDeleted = false
                }
            });

        var sut = Sut();
        var r = await sut.CreateAsync(new CreatePurchaseBillDto
        {
            SourceMode = PharmacyPurchaseBillSourceMode.DirectGrn,
            PurchaseOrderId = null,
            GoodsReceiptId = 100,
            SupplierId = 5,
            InvoiceNo = "INV-DUP",
            InvoiceDate = DateTime.UtcNow.Date,
            DiscountAmount = 0,
            GstPercent = 0,
            OtherTaxAmount = 0,
            Items = new List<PurchaseBillLineInputDto>
            {
                new() { GoodsReceiptItemId = 10, Quantity = 1, Rate = 1 },
                new() { GoodsReceiptItemId = 10, Quantity = 1, Rate = 1 }
            }
        });

        r.Success.Should().BeFalse();
        r.Message.Should().Contain("Duplicate");
    }

    [Fact]
    public async Task CreateAsync_Fails_WhenQuantityDoesNotMatchGrn()
    {
        _bills.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PhrPurchaseBill, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PhrPurchaseBill>());
        _grn.Setup(x => x.GetByIdAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(new PhrGoodsReceipt
        {
            Id = 100,
            TenantId = 1,
            FacilityId = 10,
            SupplierId = 5,
            PurchaseOrderId = null,
            IsDeleted = false
        });
        _grnItems.Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PhrGoodsReceiptItem, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PhrGoodsReceiptItem>
            {
                new()
                {
                    Id = 10,
                    GoodsReceiptId = 100,
                    LineNum = 1,
                    QuantityReceived = 5,
                    PurchaseRate = 2m,
                    IsDeleted = false
                }
            });

        var sut = Sut();
        var r = await sut.CreateAsync(new CreatePurchaseBillDto
        {
            SourceMode = PharmacyPurchaseBillSourceMode.DirectGrn,
            PurchaseOrderId = null,
            GoodsReceiptId = 100,
            SupplierId = 5,
            InvoiceNo = "INV-QTY",
            InvoiceDate = DateTime.UtcNow.Date,
            DiscountAmount = 0,
            GstPercent = 0,
            OtherTaxAmount = 0,
            Items = new List<PurchaseBillLineInputDto>
            {
                new() { GoodsReceiptItemId = 10, Quantity = 3, Rate = 2m }
            }
        });

        r.Success.Should().BeFalse();
        r.Message.Should().Contain("quantity");
    }
}
