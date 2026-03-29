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
[Route("api/v{version:apiVersion}/stock-transfer-item")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrStockTransferItem")]
public sealed class StockTransferItemController : ControllerBase
{
    private readonly IPhrStockTransferItemService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<StockTransferItemController> _logger;

    public StockTransferItemController(IPhrStockTransferItemService service, ITenantContext tenant, ILogger<StockTransferItemController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "StockTransferItem_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<StockTransferItemResponseDto>))]
    public async Task<ActionResult<BaseResponse<StockTransferItemResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "StockTransferItem_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<StockTransferItemResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<StockTransferItemResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<StockTransferItemResponseDto>>> Create([FromBody] CreateStockTransferItemDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<StockTransferItemResponseDto>>> Update(long id, [FromBody] UpdateStockTransferItemDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
