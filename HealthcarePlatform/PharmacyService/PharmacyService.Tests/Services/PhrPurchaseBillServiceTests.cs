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
}
