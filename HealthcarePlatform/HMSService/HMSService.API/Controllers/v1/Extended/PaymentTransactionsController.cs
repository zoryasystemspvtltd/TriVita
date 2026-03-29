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
/// REST API for HMS Payment transactions.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/payment-transactions")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Payment transactions")]
public sealed class PaymentTransactionsController : ControllerBase
{
    private readonly IPaymentTransactionService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PaymentTransactionsController> _logger;

    public PaymentTransactionsController(IPaymentTransactionService service, ITenantContext tenant, ILogger<PaymentTransactionsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "PaymentTransactions_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PaymentTransactionResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<PaymentTransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PaymentTransactionResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PaymentTransactions GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS PaymentTransactions GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "PaymentTransactions_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PaymentTransactionResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<PaymentTransactionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<PaymentTransactionResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? billingHeaderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PaymentTransactions GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, billingHeaderId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "PaymentTransactions_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PaymentTransactionResponseDto>))]
    public async Task<ActionResult<BaseResponse<PaymentTransactionResponseDto>>> Create([FromBody] CreatePaymentTransactionDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "PaymentTransactions_Update")]
    public async Task<ActionResult<BaseResponse<PaymentTransactionResponseDto>>> Update(long id, [FromBody] UpdatePaymentTransactionDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "PaymentTransactions_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
