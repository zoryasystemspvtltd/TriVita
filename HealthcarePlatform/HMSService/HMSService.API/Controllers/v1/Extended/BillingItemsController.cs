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
/// REST API for HMS Billing items.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/billing-items")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Billing items")]
public sealed class BillingItemsController : ControllerBase
{
    private readonly IBillingItemService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<BillingItemsController> _logger;

    public BillingItemsController(IBillingItemService service, ITenantContext tenant, ILogger<BillingItemsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "BillingItems_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<BillingItemResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<BillingItemResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<BillingItemResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS BillingItems GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS BillingItems GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "BillingItems_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<BillingItemResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<BillingItemResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<BillingItemResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? billingHeaderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS BillingItems GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, billingHeaderId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "BillingItems_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<BillingItemResponseDto>))]
    public async Task<ActionResult<BaseResponse<BillingItemResponseDto>>> Create([FromBody] CreateBillingItemDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "BillingItems_Update")]
    public async Task<ActionResult<BaseResponse<BillingItemResponseDto>>> Update(long id, [FromBody] UpdateBillingItemDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "BillingItems_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
