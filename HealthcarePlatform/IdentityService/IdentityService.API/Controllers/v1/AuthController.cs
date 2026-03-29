using Asp.Versioning;
using Healthcare.Common.Responses;
using IdentityService.Application.DTOs;
using IdentityService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityService.API.Controllers.v1;

/// <summary>Authentication: JWT access tokens and refresh tokens for TriVita services.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[SwaggerTag("Authentication")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Sign in with email and password (same as <c>/token</c>).</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Login", OperationId = "Auth_Login")]
    [SwaggerResponse(StatusCodes.Status200OK, "Token pair or error in BaseResponse body.", typeof(BaseResponse<TokenResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<TokenResponseDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<BaseResponse<TokenResponseDto>>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken) =>
        Token(request, cancellationToken);

    /// <summary>Obtains an access token and refresh token.</summary>
    [HttpPost("token")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Issue JWT", OperationId = "Auth_Token")]
    [SwaggerResponse(StatusCodes.Status200OK, "Token or error in BaseResponse body.", typeof(BaseResponse<TokenResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<TokenResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<TokenResponseDto>>> Token(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Exchanges a refresh token for a new access token and refresh token.</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Refresh access token", OperationId = "Auth_RefreshToken")]
    [ProducesResponseType(typeof(BaseResponse<TokenResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<TokenResponseDto>>> RefreshToken(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Revokes a refresh token (logout on this device).</summary>
    [HttpPost("logout")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Logout (revoke refresh token)", OperationId = "Auth_Logout")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object>>> Logout(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Starts password reset (token logged in development only).</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Forgot password", OperationId = "Auth_ForgotPassword")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object>>> ForgotPassword(
        [FromBody] ForgotPasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.ForgotPasswordAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Completes password reset with email and token from forgot-password flow.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Reset password", OperationId = "Auth_ResetPassword")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object>>> ResetPassword(
        [FromBody] ResetPasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns claims summary for the current bearer token.</summary>
    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Current user from JWT", OperationId = "Auth_Me")]
    [SwaggerResponse(StatusCodes.Status200OK, "Wrapped user summary.", typeof(BaseResponse<UserSummaryDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<UserSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<BaseResponse<UserSummaryDto>> Me()
    {
        var result = _authService.GetCurrentUser(User);
        return Ok(result);
    }
}
