using FluentAssertions;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedService.API.Controllers.v1;
using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Application.Services.FeatureExtensions;
using Xunit;

namespace SharedService.Tests.Controllers;

/// <summary>Smoke tests for 09 feature-extension controllers (mocked services).</summary>
public sealed class FeatureExtension09ControllersTests
{
    [Fact]
    public async Task FacilityServicePriceListsController_List_ReturnsOk()
    {
        var svc = new Mock<IFacilityServicePriceListService>();
        svc.Setup(s => s.ListByFacilityAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<IReadOnlyList<FacilityServicePriceListResponseDto>>.Ok(Array.Empty<FacilityServicePriceListResponseDto>()));
        var c = new FacilityServicePriceListsController(svc.Object) { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };
        var r = await c.List(1, CancellationToken.None);
        r.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task FacilityServicePriceListLinesController_Create_Calls_Service()
    {
        var svc = new Mock<IFacilityServicePriceListLineService>();
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateFacilityServicePriceListLineDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<FacilityServicePriceListLineResponseDto>.Ok(new FacilityServicePriceListLineResponseDto { Id = 1 }));
        var c = new FacilityServicePriceListLinesController(svc.Object) { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };
        await c.Create(new CreateFacilityServicePriceListLineDto { FacilityId = 1, PriceListId = 1, ServiceItemCode = "X", UnitPrice = 1 }, CancellationToken.None);
        svc.Verify(s => s.CreateAsync(It.IsAny<CreateFacilityServicePriceListLineDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CrossFacilityReportAuditsController_GetPaged_ReturnsOk()
    {
        var svc = new Mock<ICrossFacilityReportAuditService>();
        svc.Setup(s => s.GetPagedAsync(It.IsAny<PagedQuery>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<CrossFacilityReportAuditResponseDto>>.Ok(new PagedResponse<CrossFacilityReportAuditResponseDto>()));
        var c = new CrossFacilityReportAuditsController(svc.Object) { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };
        var r = await c.GetPaged(new PagedQuery(), null, CancellationToken.None);
        r.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ModuleIntegrationHandoffsController_ListByCorrelation_BadRequest_On_Service_Fail()
    {
        var svc = new Mock<IModuleIntegrationHandoffService>();
        svc.Setup(s => s.ListByCorrelationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<IReadOnlyList<ModuleIntegrationHandoffResponseDto>>.Fail("bad"));
        var c = new ModuleIntegrationHandoffsController(svc.Object) { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };
        var r = await c.ListByCorrelation("", CancellationToken.None);
        r.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task TenantOnboardingStagesController_List_ReturnsOk()
    {
        var svc = new Mock<ITenantOnboardingStageService>();
        svc.Setup(s => s.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<IReadOnlyList<TenantOnboardingStageResponseDto>>.Ok(Array.Empty<TenantOnboardingStageResponseDto>()));
        var c = new TenantOnboardingStagesController(svc.Object) { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };
        var r = await c.List(CancellationToken.None);
        r.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task LabCriticalValueEscalationsController_Create_ReturnsOk()
    {
        var svc = new Mock<ILabCriticalValueEscalationService>();
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateLabCriticalValueEscalationDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<LabCriticalValueEscalationResponseDto>.Ok(new LabCriticalValueEscalationResponseDto { Id = 1 }));
        var c = new LabCriticalValueEscalationsController(svc.Object) { ControllerContext = new() { HttpContext = new DefaultHttpContext() } };
        var r = await c.Create(new CreateLabCriticalValueEscalationDto { FacilityId = 1, ChannelCode = "SMS" }, CancellationToken.None);
        var ok = r.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse<LabCriticalValueEscalationResponseDto>>();
    }
}
