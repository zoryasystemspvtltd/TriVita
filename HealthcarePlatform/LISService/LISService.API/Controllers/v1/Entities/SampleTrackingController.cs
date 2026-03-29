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
[Route("api/v{version:apiVersion}/sample-tracking")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LisSampleTracking")]
public sealed class SampleTrackingController : ControllerBase
{
    private readonly ILisSampleTrackingService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<SampleTrackingController> _logger;

    public SampleTrackingController(ILisSampleTrackingService service, ITenantContext tenant, ILogger<SampleTrackingController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "SampleTracking_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<SampleTrackingResponseDto>))]
    public async Task<ActionResult<BaseResponse<SampleTrackingResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "SampleTracking_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<SampleTrackingResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<SampleTrackingResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<SampleTrackingResponseDto>>> Create([FromBody] CreateSampleTrackingDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<SampleTrackingResponseDto>>> Update(long id, [FromBody] UpdateSampleTrackingDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
