using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Moq;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.DTOs.Stock;
using PharmacyService.Application.Mapping;
using PharmacyService.Application.Services.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Xunit;

namespace PharmacyService.Tests.Services;

public sealed class PhrSalesBillServiceTests
{
    private readonly Mock<IRepository<PhrSalesBill>> _bills = new();
    private readonly Mock<IRepository<PhrSalesBillItem>> _items = new();
    private readonly Mock<IRepository<PhrMedicine>> _meds = new();
    private readonly Mock<IRepository<PhrMedicineBatch>> _batches = new();
    private readonly Mock<IRepository<PhrCustomer>> _customers = new();
    private readonly Mock<IPharmacyStockMovementService> _stock = new();
    private readonly Mock<IPharmacyUnitOfWork> _uow = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IFacilityTenantValidator> _fac = new();
    private readonly IMapper _mapper = new MapperConfiguration(c => c.AddProfile<PhrGeneratedMappingProfile>()).CreateMapper();
    private readonly Mock<ILogger<PhrSalesBillService>> _log = new();

    public PhrSalesBillServiceTests()
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

        _uow.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<CancellationToken, Task<BaseResponse<SalesBillResponseDto>>>>(), It.IsAny<CancellationToken>(), It.IsAny<System.Data.IsolationLevel>()))
            .Returns((Func<CancellationToken, Task<BaseResponse<SalesBillResponseDto>>> fn, CancellationToken ct, System.Data.IsolationLevel _) => fn(ct));
    }

    private PhrSalesBillService Sut() => new(
        _bills.Object,
        _items.Object,
        _meds.Object,
        _batches.Object,
        _customers.Object,
        _stock.Object,
        _uow.Object,
        _mapper,
        _tenant.Object,
        _fac.Object,
        createValidator: null,
        updateValidator: null,
        _log.Object);

    [Fact]
    public async Task CreateAsync_Fails_WhenPartyInvalid()
    {
        var sut = Sut();
        var r = await sut.CreateAsync(new CreateSalesBillDto
        {
            CustomerId = 5,
            PatientId = 9,
            SalesDate = DateTime.UtcNow.Date,
            Items = new List<SalesBillLineInputDto> { new() { MedicineId = 1, Quantity = 1, UnitPrice = 1 } }
        });

        r.Success.Should().BeFalse();
        r.Message.Should().Contain("exactly one");
    }

    [Fact]
    public async Task PostAsync_Fails_WhenFefoInsufficient()
    {
        _bills.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new PhrSalesBill
        {
            Id = 1,
            TenantId = 1,
            FacilityId = 10,
            BillNo = "SB-1",
            CustomerId = 3,
            PatientId = null,
            SalesDate = DateTime.UtcNow.Date,
            Status = PharmacySalesBillStatus.Draft,
            DiscountAmount = 0,
            GstPercent = 0,
            OtherTaxAmount = 0,
            IsDeleted = false
        });

        _items.Setup(x => x.ListAsync(It.IsAny<Expression<Func<PhrSalesBillItem, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PhrSalesBillItem>
            {
                new()
                {
                    Id = 10,
                    SalesBillId = 1,
                    LineNum = 1,
                    MedicineId = 100,
                    MedicineBatchId = null,
                    Quantity = 5,
                    UnitPrice = 10,
                    LineTotal = 50,
                    IsDeleted = false
                }
            });

        _stock.Setup(x => x.AllocateSaleFefoAsync(100, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<IReadOnlyList<StockFefoAllocation>>.Fail("Insufficient stock"));

        var sut = Sut();
        var r = await sut.PostAsync(1);

        r.Success.Should().BeFalse();
        _stock.Verify(x => x.ApplyMovementAsync(It.IsAny<StockLedgerTransactionType>(), It.IsAny<long>(), It.IsAny<long?>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<decimal>(), It.IsAny<DateTime>(), It.IsAny<decimal?>(), It.IsAny<string?>(), It.IsAny<StockLedgerMovementExtras>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void Service_UsesStockMovement()
    {
        typeof(PhrSalesBillService).GetConstructors().Single().GetParameters()
            .Select(p => p.ParameterType)
            .Should().Contain(t => t == typeof(IPharmacyStockMovementService));
    }
}
