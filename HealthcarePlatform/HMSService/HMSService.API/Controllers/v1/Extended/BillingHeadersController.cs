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
/// REST API for HMS Billing headers.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/billing-headers")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Billing headers")]
public sealed class BillingHeadersController : ControllerBase
{
    private readonly IBillingHeaderService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<BillingHeadersController> _logger;

    public BillingHeadersController(IBillingHeaderService service, ITenantContext tenant, ILogger<BillingHeadersController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "BillingHeaders_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<BillingHeaderResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<BillingHeaderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<BillingHeaderResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS BillingHeaders GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS BillingHeaders GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "BillingHeaders_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<BillingHeaderResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<BillingHeaderResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<BillingHeaderResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? visitId, [FromQuery] long? patientId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS BillingHeaders GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, visitId, patientId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "BillingHeaders_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<BillingHeaderResponseDto>))]
    public async Task<ActionResult<BaseResponse<BillingHeaderResponseDto>>> Create([FromBody] CreateBillingHeaderDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "BillingHeaders_Update")]
    public async Task<ActionResult<BaseResponse<BillingHeaderResponseDto>>> Update(long id, [FromBody] UpdateBillingHeaderDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "BillingHeaders_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
