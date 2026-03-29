using FluentAssertions;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PharmacyService.API.Controllers.v1;
using PharmacyService.Application.DTOs;
using PharmacyService.Application.Services;
using Xunit;

namespace PharmacyService.Tests.Controllers;

public sealed class InfoControllerTests
{
    [Fact]
    public void Get_ReturnsOk()
    {
        var mock = new Mock<IInfoService>();
        mock.Setup(s => s.GetInfo())
            .Returns(BaseResponse<InfoResponseDto>.Ok(new InfoResponseDto { Service = "PharmacyService" }));

        var controller = new InfoController(mock.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = controller.Get();

        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
