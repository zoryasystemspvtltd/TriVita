using FluentAssertions;
using Healthcare.Common.Responses;
using LISService.API.Controllers.v1;
using LISService.Application.DTOs;
using LISService.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LISService.Tests.Controllers;

public sealed class InfoControllerTests
{
    [Fact]
    public void Get_ReturnsOkWithBaseResponse()
    {
        var mock = new Mock<IInfoService>();
        mock.Setup(s => s.GetInfo())
            .Returns(BaseResponse<InfoResponseDto>.Ok(new InfoResponseDto
            {
                Service = "LISService",
                Module = "LIS",
                Version = "1.0"
            }));

        var controller = new InfoController(mock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = controller.Get();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<InfoResponseDto>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.Module.Should().Be("LIS");
    }
}
