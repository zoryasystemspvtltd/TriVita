$outDir = Join-Path $PSScriptRoot "..\Controllers" | Resolve-Path
$defs = @(
    @{ E = 'LabOrder'; C = 'LabOrderController'; S = 'ILisLabOrderService' },
    @{ E = 'LabOrderItem'; C = 'LabOrderItemController'; S = 'ILisLabOrderItemService' },
    @{ E = 'LabResult'; C = 'LabResultController'; S = 'ILisLabResultService' },
    @{ E = 'OrderStatusHistory'; C = 'OrderStatusHistoryController'; S = 'ILisOrderStatusHistoryService' },
    @{ E = 'ReportDetail'; C = 'ReportDetailController'; S = 'ILisReportDetailService' },
    @{ E = 'ReportHeader'; C = 'ReportHeaderController'; S = 'ILisReportHeaderService' },
    @{ E = 'ResultApproval'; C = 'ResultApprovalController'; S = 'ILisResultApprovalService' },
    @{ E = 'ResultHistory'; C = 'ResultHistoryController'; S = 'ILisResultHistoryService' },
    @{ E = 'SampleCollection'; C = 'SampleCollectionController'; S = 'ILisSampleCollectionService' },
    @{ E = 'SampleTracking'; C = 'SampleTrackingController'; S = 'ILisSampleTrackingService' },
    @{ E = 'TestMaster'; C = 'TestMasterController'; S = 'ILisTestMasterService' },
    @{ E = 'TestParameter'; C = 'TestParameterController'; S = 'ILisTestParameterService' },
    @{ E = 'SampleType'; C = 'SampleTypeController'; S = 'ILisSampleTypeService' },
    @{ E = 'TestReferenceRange'; C = 'TestReferenceRangeController'; S = 'ILisTestReferenceRangeService' },
    @{ E = 'TestParameterProfile'; C = 'TestParameterProfileController'; S = 'ILisTestParameterProfileService' },
    @{ E = 'AnalyzerResultMap'; C = 'AnalyzerResultMapController'; S = 'ILisAnalyzerResultMapService' },
    @{ E = 'SampleBarcode'; C = 'SampleBarcodeController'; S = 'ILisSampleBarcodeService' },
    @{ E = 'ReportDeliveryOtp'; C = 'ReportDeliveryOtpController'; S = 'ILisReportDeliveryOtpService' },
    @{ E = 'ReportLockState'; C = 'ReportLockStateController'; S = 'ILisReportLockStateService' }
)

foreach ($d in $defs) {
    $E = $d.E
    $C = $d.C
    $S = $d.S
    $R = "${E}ResponseDto"
    $Cr = "Create${E}Dto"
    $Up = "Update${E}Dto"
    $path = Join-Path $outDir "${C}Tests.cs"
    @"
using FluentAssertions;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.API.Controllers.v1.Entities;
using LISService.Application.DTOs.Entities;
using LISService.Application.Services.Entities;
using LISService.Tests.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LISService.Tests.Controllers;

public sealed class ${C}Tests
{
    private readonly Mock<$S> _service = new();

    private $C CreateController() => new(
        _service.Object,
        LisStandardCrudControllerTestTemplate.TenantMock().Object,
        NullLogger<$C>.Instance)
    {
        ControllerContext = LisStandardCrudControllerTestTemplate.DefaultContext
    };

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Valid()
    {
        var dto = new $R { Id = 1 };
        _service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<$R>.Ok(dto));

        var result = await CreateController().GetById(1, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeTrue();
            b.Data!.Id.Should().Be(1);
        });
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_With_Error_BaseResponse_When_Not_Found()
    {
        _service.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<$R>.Fail("Not found"));

        var result = await CreateController().GetById(99, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeFalse();
            b.Message.Should().Be("Not found");
        });
    }

    [Fact]
    public async Task GetPaged_Should_Return_Ok_When_Valid()
    {
        _service.Setup(s => s.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<$R>>.Ok(new PagedResponse<$R>
            {
                Items = Array.Empty<$R>(),
                Page = 1,
                PageSize = 10,
                TotalCount = 0
            }));

        var result = await CreateController().GetPaged(new PagedQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkPagedResponse(result, b =>
        {
            b.Success.Should().BeTrue();
            b.Data!.TotalCount.Should().Be(0);
        });
    }

    [Fact]
    public async Task Create_Should_Return_Ok_When_Valid()
    {
        var created = new $R { Id = 2 };
        _service.Setup(s => s.CreateAsync(It.IsAny<$Cr>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<$R>.Ok(created));

        var result = await CreateController().Create(new $Cr(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Data!.Id.Should().Be(2));
    }

    [Fact]
    public async Task Create_Should_Return_Ok_With_Error_BaseResponse_When_Invalid()
    {
        _service.Setup(s => s.CreateAsync(It.IsAny<$Cr>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<$R>.Fail("Invalid", new[] { "PatientId required" }));

        var result = await CreateController().Create(new $Cr(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeFalse();
            b.Errors.Should().Contain("PatientId required");
        });
    }

    [Fact]
    public async Task Update_Should_Return_Ok_When_Valid()
    {
        var updated = new $R { Id = 3 };
        _service.Setup(s => s.UpdateAsync(3, It.IsAny<$Up>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<$R>.Ok(updated));

        var result = await CreateController().Update(3, new $Up(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Data!.Id.Should().Be(3));
    }

    [Fact]
    public async Task Update_Should_Return_Ok_With_Error_BaseResponse_When_Invalid()
    {
        _service.Setup(s => s.UpdateAsync(3, It.IsAny<$Up>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<$R>.Fail("Conflict"));

        var result = await CreateController().Update(3, new $Up(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeFalse();
            b.Message.Should().Be("Conflict");
        });
    }

    [Fact]
    public async Task Delete_Should_Return_Ok_When_Valid()
    {
        _service.Setup(s => s.DeleteAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<object?>.Ok(null));

        var result = await CreateController().Delete(4, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<object?>>().Subject;
        body.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Should_Return_Ok_With_Error_BaseResponse_When_Not_Found()
    {
        _service.Setup(s => s.DeleteAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<object?>.Fail("Missing"));

        var result = await CreateController().Delete(4, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<object?>>().Subject;
        body.Success.Should().BeFalse();
    }
}
"@ | Set-Content -Path $path -Encoding utf8
    Write-Host "Wrote $path"
}
