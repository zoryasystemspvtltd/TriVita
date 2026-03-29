using FluentAssertions;
using Healthcare.Common.Responses;
using IdentityService.API.Controllers.v1;
using IdentityService.Application.DTOs;
using IdentityService.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace IdentityService.Tests;

public sealed class IdentityAdminControllerTests
{
    private readonly Mock<IIdentityAdminService> _admin = new();

    private IdentityAdminController CreateController() => new(_admin.Object)
    {
        ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
    };

    [Fact]
    public async Task CreateUser_Should_Return_Ok_When_Valid()
    {
        _admin.Setup(a => a.CreateUserAsync(It.IsAny<CreateIdentityUserRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<long>.Ok(99));

        var result = await CreateController().CreateUser(new CreateIdentityUserRequestDto(), CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<long>>().Subject;
        body.Success.Should().BeTrue();
        body.Data.Should().Be(99);
    }

    [Fact]
    public async Task CreateUser_Should_Return_Ok_With_Error_When_Service_Fails()
    {
        _admin.Setup(a => a.CreateUserAsync(It.IsAny<CreateIdentityUserRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<long>.Fail("Email exists"));

        var result = await CreateController().CreateUser(new CreateIdentityUserRequestDto(), CancellationToken.None);

        var body = result.Result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<BaseResponse<long>>().Subject;
        body.Success.Should().BeFalse();
        body.Message.Should().Be("Email exists");
    }

    [Fact]
    public async Task AssignUserRoles_Should_Return_Ok_When_Valid()
    {
        _admin.Setup(a => a.AssignUserRolesAsync(1, It.IsAny<AssignUserRolesRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<object>.Ok(new object()));

        var result = await CreateController().AssignUserRoles(1, new AssignUserRolesRequestDto(), CancellationToken.None);

        var body = result.Result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<BaseResponse<object>>().Subject;
        body.Success.Should().BeTrue();
    }

    [Fact]
    public async Task AssignUserFacilities_Should_Return_Ok_With_Error_When_Invalid()
    {
        _admin.Setup(a => a.AssignUserFacilitiesAsync(2, It.IsAny<AssignUserFacilitiesRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<object>.Fail("Unknown facility"));

        var result = await CreateController().AssignUserFacilities(2, new AssignUserFacilitiesRequestDto(), CancellationToken.None);

        var body = result.Result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<BaseResponse<object>>().Subject;
        body.Success.Should().BeFalse();
    }

    [Fact]
    public async Task CreateRole_Should_Return_Ok_When_Valid()
    {
        _admin.Setup(a => a.CreateRoleAsync(It.IsAny<CreateRoleRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<long>.Ok(5));

        var result = await CreateController().CreateRole(new CreateRoleRequestDto(), CancellationToken.None);

        var body = result.Result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<BaseResponse<long>>().Subject;
        body.Data.Should().Be(5);
    }

    [Fact]
    public async Task AssignRolePermissions_Should_Return_Ok_When_Valid()
    {
        _admin.Setup(a => a.AssignRolePermissionsAsync(3, It.IsAny<AssignRolePermissionsRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<object>.Ok(new object()));

        var result = await CreateController().AssignRolePermissions(3, new AssignRolePermissionsRequestDto(), CancellationToken.None);

        var body = result.Result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<BaseResponse<object>>().Subject;
        body.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreatePermission_Should_Return_Ok_When_Valid()
    {
        _admin.Setup(a => a.CreatePermissionAsync(It.IsAny<CreatePermissionRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<long>.Ok(7));

        var result = await CreateController().CreatePermission(new CreatePermissionRequestDto(), CancellationToken.None);

        var body = result.Result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<BaseResponse<long>>().Subject;
        body.Data.Should().Be(7);
    }
}
