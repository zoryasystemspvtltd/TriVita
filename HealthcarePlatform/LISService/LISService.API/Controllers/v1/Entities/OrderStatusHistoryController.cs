using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.Application.DTOs.Entities;
using LISService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LISService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/order-status-history")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LisOrderStatusHistory")]
public sealed class OrderStatusHistoryController : ControllerBase
{
    private readonly ILisOrderStatusHistoryService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<OrderStatusHistoryController> _logger;

    public OrderStatusHistoryController(ILisOrderStatusHistoryService service, ITenantContext tenant, ILogger<OrderStatusHistoryController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "OrderStatusHistory_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<OrderStatusHistoryResponseDto>))]
    public async Task<ActionResult<BaseResponse<OrderStatusHistoryResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "OrderStatusHistory_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<OrderStatusHistoryResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<OrderStatusHistoryResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<OrderStatusHistoryResponseDto>>> Create([FromBody] CreateOrderStatusHistoryDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<OrderStatusHistoryResponseDto>>> Update(long id, [FromBody] UpdateOrderStatusHistoryDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
