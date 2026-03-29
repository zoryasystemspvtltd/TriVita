using System.Security.Claims;
using Healthcare.Common.Responses;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Services;

public interface IAuthService
{
    Task<BaseResponse<TokenResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<object>> LogoutAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<object>> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<object>> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default);

    BaseResponse<UserSummaryDto> GetCurrentUser(ClaimsPrincipal user);
}
