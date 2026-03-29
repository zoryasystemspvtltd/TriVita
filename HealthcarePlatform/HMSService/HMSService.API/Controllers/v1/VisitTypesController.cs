using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.VisitTypes;
using HMSService.Application.Services.Extended;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1;

/// <summary>
/// HMS visit type catalog (OPD/ER/follow-up).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/visit-types")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Visit types")]
public sealed class VisitTypesController : ControllerBase
{
    private readonly IVisitTypeService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<VisitTypesController> _logger;

    public VisitTypesController(
        IVisitTypeService service,
        ITenantContext tenant,
        ILogger<VisitTypesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets a visit type by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get visit type by id", OperationId = "VisitTypes_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<VisitTypeResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<VisitTypeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<VisitTypeResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS VisitTypes GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>Lists visit types (paged).</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List visit types (paged)", OperationId = "VisitTypes_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<VisitTypeResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<VisitTypeResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<VisitTypeResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS VisitTypes GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a visit type (tenant-scoped; optional facility).</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create visit type", OperationId = "VisitTypes_Create")]
    public async Task<ActionResult<BaseResponse<VisitTypeResponseDto>>> Create(
        [FromBody] CreateVisitTypeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates a visit type.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update visit type", OperationId = "VisitTypes_Update")]
    public async Task<ActionResult<BaseResponse<VisitTypeResponseDto>>> Update(
        long id,
        [FromBody] UpdateVisitTypeDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a visit type.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete visit type (soft)", OperationId = "VisitTypes_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
