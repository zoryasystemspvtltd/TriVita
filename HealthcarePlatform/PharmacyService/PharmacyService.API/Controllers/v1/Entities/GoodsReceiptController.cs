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
[Route("api/v{version:apiVersion}/goods-receipt")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrGoodsReceipt")]
public sealed class GoodsReceiptController : ControllerBase
{
    private readonly IPhrGoodsReceiptService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<GoodsReceiptController> _logger;

    public GoodsReceiptController(IPhrGoodsReceiptService service, ITenantContext tenant, ILogger<GoodsReceiptController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "GoodsReceipt_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<GoodsReceiptResponseDto>))]
    public async Task<ActionResult<BaseResponse<GoodsReceiptResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "GoodsReceipt_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<GoodsReceiptResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<GoodsReceiptResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<GoodsReceiptResponseDto>>> Create([FromBody] CreateGoodsReceiptDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<GoodsReceiptResponseDto>>> Update(long id, [FromBody] UpdateGoodsReceiptDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
