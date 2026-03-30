using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using HMSService.Application.DTOs.Gap;
using HMSService.Application.Services.Gap;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1.Extended;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ipd/housekeeping-statuses")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS IPD Housekeeping")]
public sealed class HousekeepingStatusesController : ControllerBase
{
    private readonly IHousekeepingStatusService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<HousekeepingStatusesController> _logger;

    public HousekeepingStatusesController(
        IHousekeepingStatusService service,
        ITenantContext tenant,
        ILogger<HousekeepingStatusesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "Housekeeping_GetById")]
    public async Task<ActionResult<BaseResponse<HousekeepingStatusResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS Housekeeping GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "Housekeeping_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<HousekeepingStatusResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? bedId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, bedId, cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "Housekeeping_Create")]
    public async Task<ActionResult<BaseResponse<HousekeepingStatusResponseDto>>> Create(
        [FromBody] CreateHousekeepingStatusDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(OperationId = "Housekeeping_Update")]
    public async Task<ActionResult<BaseResponse<HousekeepingStatusResponseDto>>> Update(
        long id,
        [FromBody] UpdateHousekeepingStatusDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(OperationId = "Housekeeping_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        return Ok(await _service.DeleteAsync(id, cancellationToken));
    }
}
