using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using Swashbuckle.AspNetCore.Annotations;

namespace SharedService.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/facilities")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("Enterprise hierarchy")]
public sealed class FacilitiesController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public FacilitiesController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List facilities (optional business unit filter)", OperationId = "Shared_Facilities_List")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<FacilityResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<FacilityResponseDto>>>> List(
        [FromQuery] long? businessUnitId,
        CancellationToken cancellationToken) =>
        Ok(await _facilityService.ListAsync(businessUnitId, cancellationToken));

    /// <summary>Used by other microservices to validate <c>FacilityId</c> and load enterprise chain.</summary>
    [HttpGet("{id:long}/hierarchy-context")]
    [SwaggerOperation(Summary = "Get facility hierarchy context for tenant", OperationId = "Shared_Facilities_GetHierarchyContext")]
    [ProducesResponseType(typeof(BaseResponse<FacilityHierarchyContextDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityHierarchyContextDto>>> GetHierarchyContext(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await _facilityService.GetHierarchyContextAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get facility by id", OperationId = "Shared_Facilities_GetById")]
    [ProducesResponseType(typeof(BaseResponse<FacilityResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _facilityService.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create facility", OperationId = "Shared_Facilities_Create")]
    [ProducesResponseType(typeof(BaseResponse<FacilityResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityResponseDto>>> Create(
        [FromBody] CreateFacilityDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _facilityService.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update facility", OperationId = "Shared_Facilities_Update")]
    [ProducesResponseType(typeof(BaseResponse<FacilityResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityResponseDto>>> Update(
        long id,
        [FromBody] UpdateFacilityDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _facilityService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete facility", OperationId = "Shared_Facilities_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _facilityService.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
