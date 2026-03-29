using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using IdentityService.Application.DTOs;
using IdentityService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IdentityService.API.Controllers.v1;

/// <summary>User, role, and permission administration (requires <c>identity.admin</c>).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/identity-admin")]
[RequirePermission(TriVitaPermissions.IdentityAdmin)]
[SwaggerTag("Identity administration")]
public sealed class IdentityAdminController : ControllerBase
{
    private readonly IIdentityAdminService _admin;

    public IdentityAdminController(IIdentityAdminService admin)
    {
        _admin = admin;
    }

    [HttpPost("users")]
    [SwaggerOperation(Summary = "Create user", OperationId = "IdentityAdmin_CreateUser")]
    [ProducesResponseType(typeof(BaseResponse<long>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<long>>> CreateUser(
        [FromBody] CreateIdentityUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _admin.CreateUserAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("users/{userId:long}/roles")]
    [SwaggerOperation(Summary = "Assign roles to user", OperationId = "IdentityAdmin_AssignUserRoles")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object>>> AssignUserRoles(
        long userId,
        [FromBody] AssignUserRolesRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _admin.AssignUserRolesAsync(userId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("users/{userId:long}/facilities")]
    [SwaggerOperation(Summary = "Assign facility grants", OperationId = "IdentityAdmin_AssignUserFacilities")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object>>> AssignUserFacilities(
        long userId,
        [FromBody] AssignUserFacilitiesRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _admin.AssignUserFacilitiesAsync(userId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("roles")]
    [SwaggerOperation(Summary = "Create role", OperationId = "IdentityAdmin_CreateRole")]
    [ProducesResponseType(typeof(BaseResponse<long>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<long>>> CreateRole(
        [FromBody] CreateRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _admin.CreateRoleAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("roles/{roleId:long}/permissions")]
    [SwaggerOperation(Summary = "Assign permissions to role", OperationId = "IdentityAdmin_AssignRolePermissions")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object>>> AssignRolePermissions(
        long roleId,
        [FromBody] AssignRolePermissionsRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _admin.AssignRolePermissionsAsync(roleId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("permissions")]
    [SwaggerOperation(Summary = "Create permission", OperationId = "IdentityAdmin_CreatePermission")]
    [ProducesResponseType(typeof(BaseResponse<long>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<long>>> CreatePermission(
        [FromBody] CreatePermissionRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _admin.CreatePermissionAsync(request, cancellationToken);
        return Ok(result);
    }
}
