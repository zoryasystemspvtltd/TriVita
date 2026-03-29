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
[Route("api/v{version:apiVersion}/expiry-tracking")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrExpiryTracking")]
public sealed class ExpiryTrackingController : ControllerBase
{
    private readonly IPhrExpiryTrackingService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ExpiryTrackingController> _logger;

    public ExpiryTrackingController(IPhrExpiryTrackingService service, ITenantContext tenant, ILogger<ExpiryTrackingController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ExpiryTracking_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<ExpiryTrackingResponseDto>))]
    public async Task<ActionResult<BaseResponse<ExpiryTrackingResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ExpiryTracking_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<ExpiryTrackingResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<ExpiryTrackingResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ExpiryTrackingResponseDto>>> Create([FromBody] CreateExpiryTrackingDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ExpiryTrackingResponseDto>>> Update(long id, [FromBody] UpdateExpiryTrackingDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
