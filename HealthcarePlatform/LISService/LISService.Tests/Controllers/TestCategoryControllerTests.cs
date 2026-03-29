using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using LISService.API.Controllers.v1.Entities;
using LISService.Application.DTOs.Entities;
using LISService.Application.Services.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LISService.Tests.Controllers;

public sealed class TestCategoryControllerTests
{
    private readonly Mock<ILisTestCategoryService> _service = new();

    [Fact]
    public async Task GetById_ReturnsOk()
    {
        _service
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestCategoryResponseDto>.Ok(new TestCategoryResponseDto
            {
                Id = 1,
                TenantId = 1,
                CategoryCode = "LAB",
                CategoryName = "Laboratory"
            }));

        var controller = new TestCategoryController(
            _service.Object,
            Mock.Of<ITenantContext>(),
            NullLogger<TestCategoryController>.Instance);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.GetById(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<TestCategoryResponseDto>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.CategoryCode.Should().Be("LAB");
    }
}
