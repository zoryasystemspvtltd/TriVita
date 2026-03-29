using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Extended;
using HMSService.Application.Services.Extended;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1.Extended;

/// <summary>
/// REST API for HMS Prescription items.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/prescription-items")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Prescription items")]
public sealed class PrescriptionItemsController : ControllerBase
{
    private readonly IPrescriptionItemService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PrescriptionItemsController> _logger;

    public PrescriptionItemsController(IPrescriptionItemService service, ITenantContext tenant, ILogger<PrescriptionItemsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "PrescriptionItems_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PrescriptionItemResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<PrescriptionItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PrescriptionItemResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PrescriptionItems GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS PrescriptionItems GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "PrescriptionItems_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PrescriptionItemResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<PrescriptionItemResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<PrescriptionItemResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? prescriptionId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PrescriptionItems GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, prescriptionId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "PrescriptionItems_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PrescriptionItemResponseDto>))]
    public async Task<ActionResult<BaseResponse<PrescriptionItemResponseDto>>> Create([FromBody] CreatePrescriptionItemDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "PrescriptionItems_Update")]
    public async Task<ActionResult<BaseResponse<PrescriptionItemResponseDto>>> Update(long id, [FromBody] UpdatePrescriptionItemDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "PrescriptionItems_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
