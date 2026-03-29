using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.Extensions.Logging;
using LMSService.API.Controllers.v1.Entities;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMSService.Tests.Controllers;

public sealed class IamRolesControllerTests
{
    [Fact]
    public async Task GetById_Should_Return_Ok_With_BaseResponse()
    {
        var dto = new IamRoleResponseDto { Id = 1, RoleCode = "LAB_TECH", RoleName = "Lab Tech" };
        var service = new Mock<IIamRoleService>();
        service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<IamRoleResponseDto>.Ok(dto));

        var controller = new IamRolesController(service.Object, Mock.Of<ITenantContext>(), Mock.Of<ILogger<IamRolesController>>())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        var result = await controller.GetById(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<IamRoleResponseDto>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.RoleCode.Should().Be("LAB_TECH");
    }

    [Fact]
    public async Task GetPaged_Should_Return_Ok()
    {
        var service = new Mock<IIamRoleService>();
        service.Setup(s => s.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<IamRoleResponseDto>>.Ok(new PagedResponse<IamRoleResponseDto>
            {
                Items = Array.Empty<IamRoleResponseDto>(),
                Page = 1,
                PageSize = 10,
                TotalCount = 0
            }));

        var controller = new IamRolesController(service.Object, Mock.Of<ITenantContext>(), Mock.Of<ILogger<IamRolesController>>())
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        var result = await controller.GetPaged(new PagedQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<PagedResponse<IamRoleResponseDto>>>().Subject;
        body.Success.Should().BeTrue();
    }
}
