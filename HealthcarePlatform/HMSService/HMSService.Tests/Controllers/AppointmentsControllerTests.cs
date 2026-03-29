using FluentAssertions;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.API.Controllers.v1;
using HMSService.Application.DTOs.Appointments;
using HMSService.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HMSService.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="AppointmentsController"/> using a mocked <see cref="IAppointmentService"/>.
/// </summary>
public sealed class AppointmentsControllerTests
{
    private readonly Mock<IAppointmentService> _service = new();

    [Fact]
    public async Task GetById_ReturnsOk_WithBaseResponse()
    {
        var dto = new AppointmentResponseDto { Id = 1, AppointmentNo = "APT-X" };
        _service
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<AppointmentResponseDto>.Ok(dto));

        var controller = new AppointmentsController(_service.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var actionResult = await controller.GetById(1, CancellationToken.None);

        var ok = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<AppointmentResponseDto>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(1);
    }

    [Fact]
    public async Task Create_PostsToService_ReturnsOk()
    {
        var created = new AppointmentResponseDto { Id = 2, AppointmentNo = "APT-NEW" };
        _service
            .Setup(s => s.CreateAsync(It.IsAny<CreateAppointmentDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<AppointmentResponseDto>.Ok(created));

        var controller = new AppointmentsController(_service.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var input = new CreateAppointmentDto
        {
            PatientId = 1,
            DoctorId = 1,
            DepartmentId = 1,
            AppointmentStatusValueId = 1,
            ScheduledStartOn = DateTime.UtcNow
        };

        var actionResult = await controller.Create(input, CancellationToken.None);

        var ok = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<AppointmentResponseDto>>().Subject;
        body.Data!.AppointmentNo.Should().Be("APT-NEW");
    }

    [Fact]
    public async Task GetPaged_PassesQueryToService()
    {
        _service
            .Setup(s => s.GetPagedAsync(It.IsAny<PagedQuery>(), 5, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<AppointmentResponseDto>>.Ok(new PagedResponse<AppointmentResponseDto>
            {
                Items = Array.Empty<AppointmentResponseDto>(),
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            }));

        var controller = new AppointmentsController(_service.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var query = new PagedQuery { Page = 1, PageSize = 20 };
        await controller.GetPaged(query, 5, null, null, null, CancellationToken.None);

        _service.Verify(
            s => s.GetPagedAsync(query, 5, null, null, null, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
