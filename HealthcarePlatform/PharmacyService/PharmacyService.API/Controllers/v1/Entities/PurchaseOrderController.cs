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
[Route("api/v{version:apiVersion}/purchase-order")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrPurchaseOrder")]
public sealed class PurchaseOrderController : ControllerBase
{
    private readonly IPhrPurchaseOrderService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PurchaseOrderController> _logger;

    public PurchaseOrderController(IPhrPurchaseOrderService service, ITenantContext tenant, ILogger<PurchaseOrderController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "PurchaseOrder_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PurchaseOrderResponseDto>))]
    public async Task<ActionResult<BaseResponse<PurchaseOrderResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "PurchaseOrder_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PurchaseOrderResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<PurchaseOrderResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PurchaseOrderResponseDto>>> Create([FromBody] CreatePurchaseOrderDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PurchaseOrderResponseDto>>> Update(long id, [FromBody] UpdatePurchaseOrderDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
