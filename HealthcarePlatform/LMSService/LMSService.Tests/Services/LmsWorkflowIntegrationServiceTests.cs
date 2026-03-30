using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using LMSService.Application.Services.Workflow;
using LMSService.Domain.Entities;
using LMSService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LMSService.Tests.Services;

public sealed class LmsWorkflowIntegrationServiceTests
{
    private readonly Mock<IRepository<LmsLabSampleBarcode>> _barcodes = new();
    private readonly Mock<IRepository<LmsLabTestBookingItem>> _items = new();
    private readonly Mock<IRepository<LmsLabTestBooking>> _bookings = new();
    private readonly Mock<IRepository<LmsCatalogTest>> _tests = new();
    private readonly Mock<IRepository<LmsCatalogTestParameterMap>> _maps = new();
    private readonly Mock<IRepository<LmsCatalogParameter>> _parameters = new();
    private readonly Mock<IRepository<LmsEquipmentTestMaster>> _equipmentTests = new();
    private readonly Mock<ITenantContext> _tenant = new();

    public LmsWorkflowIntegrationServiceTests()
    {
        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.FacilityId).Returns(10L);
    }

    private LmsWorkflowIntegrationService CreateSut() =>
        new(
            _barcodes.Object,
            _items.Object,
            _bookings.Object,
            _tests.Object,
            _maps.Object,
            _parameters.Object,
            _equipmentTests.Object,
            _tenant.Object,
            NullLogger<LmsWorkflowIntegrationService>.Instance);

    [Fact]
    public async Task ResolveBarcodeAsync_WhenMissing_ReturnsFail()
    {
        _barcodes.Setup(b => b.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<LmsLabSampleBarcode, bool>>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LmsLabSampleBarcode>());

        var sut = CreateSut();
        var result = await sut.ResolveBarcodeAsync("X-404");

        result.Success.Should().BeFalse();
    }
}
