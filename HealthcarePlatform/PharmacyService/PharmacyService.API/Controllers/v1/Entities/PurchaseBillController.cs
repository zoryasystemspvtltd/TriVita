using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/purchase-bill")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrPurchaseBill")]
public sealed class PurchaseBillController : ControllerBase
{
    private readonly IPhrPurchaseBillService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PurchaseBillController> _logger;

    public PurchaseBillController(IPhrPurchaseBillService service, ITenantContext tenant, ILogger<PurchaseBillController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "PurchaseBill_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PurchaseBillResponseDto>))]
    public async Task<ActionResult<BaseResponse<PurchaseBillResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("PurchaseBill GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "PurchaseBill_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PurchaseBillResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<PurchaseBillResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PurchaseBillResponseDto>>> Create([FromBody] CreatePurchaseBillDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PurchaseBillResponseDto>>> Update(long id, [FromBody] UpdatePurchaseBillDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));

    [HttpPost("{id:long}/post")]
    [SwaggerOperation(OperationId = "PurchaseBill_Post")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PurchaseBillResponseDto>))]
    public async Task<ActionResult<BaseResponse<PurchaseBillResponseDto>>> Post(long id, CancellationToken ct)
        => Ok(await _service.PostAsync(id, ct));
}
