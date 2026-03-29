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
/// REST API for HMS Payment modes.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payment-modes")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Payment modes")]
public sealed class PaymentModesController : ControllerBase
{
    private readonly IPaymentModeService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PaymentModesController> _logger;

    public PaymentModesController(IPaymentModeService service, ITenantContext tenant, ILogger<PaymentModesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "PaymentModes_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PaymentModeResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<PaymentModeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PaymentModeResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PaymentModes GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS PaymentModes GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "PaymentModes_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PaymentModeResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<PaymentModeResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<PaymentModeResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PaymentModes GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "PaymentModes_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PaymentModeResponseDto>))]
    public async Task<ActionResult<BaseResponse<PaymentModeResponseDto>>> Create([FromBody] CreatePaymentModeDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "PaymentModes_Update")]
    public async Task<ActionResult<BaseResponse<PaymentModeResponseDto>>> Update(long id, [FromBody] UpdatePaymentModeDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "PaymentModes_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
