using FluentAssertions;
using Healthcare.Common.Responses;
using LMSService.API.Controllers.v1;
using LMSService.Application.DTOs;
using LMSService.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMSService.Tests.Controllers;

public sealed class InfoControllerTests
{
    [Fact]
    public void Get_ReturnsOk()
    {
        var mock = new Mock<IInfoService>();
        mock.Setup(s => s.GetInfo())
            .Returns(BaseResponse<InfoResponseDto>.Ok(new InfoResponseDto { Service = "LMSService", Module = "LMS" }));

        var controller = new InfoController(mock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = controller.Get();

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
