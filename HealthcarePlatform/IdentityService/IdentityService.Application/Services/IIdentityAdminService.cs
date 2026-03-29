using Healthcare.Common.Responses;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Services;

public interface IIdentityAdminService
{
    Task<BaseResponse<long>> CreateUserAsync(CreateIdentityUserRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<object>> AssignUserRolesAsync(long userId, AssignUserRolesRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<object>> AssignUserFacilitiesAsync(long userId, AssignUserFacilitiesRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<long>> CreateRoleAsync(CreateRoleRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<long>> CreatePermissionAsync(CreatePermissionRequestDto request, CancellationToken cancellationToken = default);

    Task<BaseResponse<object>> AssignRolePermissionsAsync(long roleId, AssignRolePermissionsRequestDto request, CancellationToken cancellationToken = default);
}
