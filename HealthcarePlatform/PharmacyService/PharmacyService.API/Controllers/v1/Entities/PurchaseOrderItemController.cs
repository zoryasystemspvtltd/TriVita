using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/purchase-order-item")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrPurchaseOrderItem")]
public sealed class PurchaseOrderItemController : ControllerBase
{
    private readonly IPhrPurchaseOrderItemService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PurchaseOrderItemController> _logger;

    public PurchaseOrderItemController(IPhrPurchaseOrderItemService service, ITenantContext tenant, ILogger<PurchaseOrderItemController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "PurchaseOrderItem_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PurchaseOrderItemResponseDto>))]
    public async Task<ActionResult<BaseResponse<PurchaseOrderItemResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "PurchaseOrderItem_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PurchaseOrderItemResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<PurchaseOrderItemResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PurchaseOrderItemResponseDto>>> Create([FromBody] CreatePurchaseOrderItemDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PurchaseOrderItemResponseDto>>> Update(long id, [FromBody] UpdatePurchaseOrderItemDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
