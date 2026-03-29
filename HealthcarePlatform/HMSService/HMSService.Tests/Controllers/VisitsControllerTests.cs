using FluentAssertions;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.API.Controllers.v1;
using HMSService.Application.DTOs.Visits;
using HMSService.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HMSService.Tests.Controllers;

public sealed class VisitsControllerTests
{
    private readonly Mock<IVisitService> _service = new();

    [Fact]
    public async Task GetById_Should_Return_Ok_With_BaseResponse()
    {
        var dto = new VisitResponseDto { Id = 1, VisitNo = "VIS-1" };
        _service
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<VisitResponseDto>.Ok(dto));

        var controller = new VisitsController(_service.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        var actionResult = await controller.GetById(1, CancellationToken.None);

        var ok = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<VisitResponseDto>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.VisitNo.Should().Be("VIS-1");
    }

    [Fact]
    public async Task GetPaged_Should_Pass_Filters_To_Service()
    {
        _service
            .Setup(s => s.GetPagedAsync(
                It.IsAny<PagedQuery>(),
                7,
                8,
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<VisitResponseDto>>.Ok(new PagedResponse<VisitResponseDto>
            {
                Items = Array.Empty<VisitResponseDto>(),
                Page = 1,
                PageSize = 10,
                TotalCount = 0
            }));

        var controller = new VisitsController(_service.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        var query = new PagedQuery { Page = 1, PageSize = 10 };
        await controller.GetPaged(query, 7, 8, null, null, CancellationToken.None);

        _service.Verify(
            s => s.GetPagedAsync(
                It.Is<PagedQuery>(q => q.Page == 1 && q.PageSize == 10),
                7,
                8,
                null,
                null,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Create_Should_Return_Ok_When_Service_Succeeds()
    {
        var created = new VisitResponseDto { Id = 2, VisitNo = "VIS-NEW" };
        _service
            .Setup(s => s.CreateAsync(It.IsAny<CreateVisitDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<VisitResponseDto>.Ok(created));

        var controller = new VisitsController(_service.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        var input = new CreateVisitDto
        {
            PatientId = 1,
            DoctorId = 2,
            DepartmentId = 3,
            VisitTypeId = 4,
            VisitStartOn = DateTime.UtcNow,
            CurrentStatusReferenceValueId = 5
        };

        var actionResult = await controller.Create(input, CancellationToken.None);

        var ok = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<VisitResponseDto>>().Subject;
        body.Data!.VisitNo.Should().Be("VIS-NEW");
    }
}
