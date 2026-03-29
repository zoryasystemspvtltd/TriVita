using FluentAssertions;
using Healthcare.Common.Responses;
using IdentityService.API.Controllers.v1;
using IdentityService.Application.DTOs;
using IdentityService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace IdentityService.Tests;

public sealed class AuthControllerTests
{
    [Fact]
    public async Task Login_delegates_to_auth_service()
    {
        var auth = new Mock<IAuthService>();
        auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TokenResponseDto>.Ok(new TokenResponseDto
            {
                AccessToken = "t",
                RefreshToken = "r",
            }));

        var controller = new AuthController(auth.Object);
        var result = await controller.Login(new LoginRequestDto { Email = "a@b.c", Password = "p", TenantId = 1 }, CancellationToken.None);

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var body = ok!.Value as BaseResponse<TokenResponseDto>;
        body!.Data!.AccessToken.Should().Be("t");
    }
}
