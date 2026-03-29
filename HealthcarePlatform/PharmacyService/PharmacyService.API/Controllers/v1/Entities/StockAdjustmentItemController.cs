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
[Route("api/v{version:apiVersion}/stock-adjustment-item")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrStockAdjustmentItem")]
public sealed class StockAdjustmentItemController : ControllerBase
{
    private readonly IPhrStockAdjustmentItemService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<StockAdjustmentItemController> _logger;

    public StockAdjustmentItemController(IPhrStockAdjustmentItemService service, ITenantContext tenant, ILogger<StockAdjustmentItemController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "StockAdjustmentItem_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<StockAdjustmentItemResponseDto>))]
    public async Task<ActionResult<BaseResponse<StockAdjustmentItemResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "StockAdjustmentItem_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<StockAdjustmentItemResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<StockAdjustmentItemResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<StockAdjustmentItemResponseDto>>> Create([FromBody] CreateStockAdjustmentItemDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<StockAdjustmentItemResponseDto>>> Update(long id, [FromBody] UpdateStockAdjustmentItemDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
