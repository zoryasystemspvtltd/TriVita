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
[Route("api/v{version:apiVersion}/pharmacy-sales-item")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrPharmacySalesItem")]
public sealed class PharmacySalesItemController : ControllerBase
{
    private readonly IPhrPharmacySalesItemService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PharmacySalesItemController> _logger;

    public PharmacySalesItemController(IPhrPharmacySalesItemService service, ITenantContext tenant, ILogger<PharmacySalesItemController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "PharmacySalesItem_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PharmacySalesItemResponseDto>))]
    public async Task<ActionResult<BaseResponse<PharmacySalesItemResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "PharmacySalesItem_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PharmacySalesItemResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<PharmacySalesItemResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PharmacySalesItemResponseDto>>> Create([FromBody] CreatePharmacySalesItemDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PharmacySalesItemResponseDto>>> Update(long id, [FromBody] UpdatePharmacySalesItemDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
