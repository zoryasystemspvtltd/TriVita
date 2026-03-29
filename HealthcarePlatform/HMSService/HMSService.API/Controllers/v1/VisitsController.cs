using Asp.Versioning;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Visits;
using HMSService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1;

/// <summary>
/// OPD visits linked to appointments when applicable.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/visits")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("Visits")]
public sealed class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitsController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    /// <summary>Gets a visit by identifier.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get visit by id", OperationId = "Visits_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Wrapped result.", typeof(BaseResponse<VisitResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<VisitResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<VisitResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _visitService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>Lists visits with pagination and filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List visits (paged)", OperationId = "Visits_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paged list.", typeof(BaseResponse<PagedResponse<VisitResponseDto>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<VisitResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<PagedResponse<VisitResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? patientId,
        [FromQuery] long? doctorId,
        [FromQuery] DateTime? visitFrom,
        [FromQuery] DateTime? visitTo,
        CancellationToken cancellationToken)
    {
        var result = await _visitService.GetPagedAsync(
            query,
            patientId,
            doctorId,
            visitFrom,
            visitTo,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a visit (requires facility context).</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create visit", OperationId = "Visits_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "Created or validation error in body.", typeof(BaseResponse<VisitResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<VisitResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<VisitResponseDto>>> Create(
        [FromBody] CreateVisitDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _visitService.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates a visit.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update visit", OperationId = "Visits_Update")]
    [SwaggerResponse(StatusCodes.Status200OK, "Updated or error in body.", typeof(BaseResponse<VisitResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<VisitResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<VisitResponseDto>>> Update(
        long id,
        [FromBody] UpdateVisitDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _visitService.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a visit.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete visit (soft)", OperationId = "Visits_Delete")]
    [SwaggerResponse(StatusCodes.Status200OK, "Deleted or error in body.", typeof(BaseResponse<object>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _visitService.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
