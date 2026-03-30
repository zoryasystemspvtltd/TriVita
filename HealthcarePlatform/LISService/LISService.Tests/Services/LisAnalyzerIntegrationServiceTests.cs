using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using LISService.Application.Abstractions;
using LISService.Application.DTOs.Analyzer;
using LISService.Application.Services.Analyzer;
using LISService.Domain.Entities;
using LISService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LISService.Tests.Services;

public sealed class LisAnalyzerIntegrationServiceTests
{
    private readonly Mock<ILmsWorkflowApiClient> _lms = new();
    private readonly Mock<IRepository<LisAnalyzerResultHeader>> _headers = new();
    private readonly Mock<IRepository<LisAnalyzerResultLine>> _lines = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IValidator<AnalyzerResultIngestDto>> _validator = new();
    private readonly Mock<ILisNotificationHelper> _notifications = new();

    public LisAnalyzerIntegrationServiceTests()
    {
        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.FacilityId).Returns(10L);
        _tenant.SetupGet(t => t.UserId).Returns(99L);

        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<AnalyzerResultIngestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private LisAnalyzerIntegrationService CreateSut() =>
        new(
            _lms.Object,
            _headers.Object,
            _lines.Object,
            _tenant.Object,
            _validator.Object,
            _notifications.Object,
            NullLogger<LisAnalyzerIntegrationService>.Instance);

    [Fact]
    public async Task QueryTestAsync_WhenFacilityMissing_ReturnsFail()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);
        var sut = CreateSut();

        var result = await sut.QueryTestAsync("BC1");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("FacilityId");
    }

    [Fact]
    public async Task QueryTestAsync_WhenLmsReturnsData_MapsEquipmentCodes()
    {
        _lms.Setup(c => c.ResolveBarcodeAsync("BC1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<LmsBarcodeResolutionClientDto>.Ok(new LmsBarcodeResolutionClientDto
            {
                FacilityId = 10,
                BarcodeValue = "BC1",
                CatalogTestId = 5,
                TestCode = "CBC",
                TestName = "CBC",
                TestBookingItemId = 20,
                LabTestBookingId = 30,
                PatientId = 40,
                EquipmentAssays = new List<LmsEquipmentAssayClientDto>
                {
                    new() { EquipmentId = 7, EquipmentAssayCode = "ASSAY-1" }
                }
            }));

        var sut = CreateSut();
        var result = await sut.QueryTestAsync("BC1");

        result.Success.Should().BeTrue();
        result.Data!.EquipmentTestCodes.Should().ContainSingle().Which.Should().Be("ASSAY-1");
    }
}
