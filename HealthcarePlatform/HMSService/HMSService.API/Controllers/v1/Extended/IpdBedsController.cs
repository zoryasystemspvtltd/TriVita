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
[Route("api/v{version:apiVersion}/ipd/beds")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS IPD Beds")]
public sealed class IpdBedsController : ControllerBase
{
    private readonly IBedService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IpdBedsController> _logger;

    public IpdBedsController(IBedService service, ITenantContext tenant, ILogger<IpdBedsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IpdBeds_GetById")]
    public async Task<ActionResult<BaseResponse<BedResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS Bed GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IpdBeds_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<BedResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? wardId,
        [FromQuery] bool? onlyAvailable,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, wardId, onlyAvailable, cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "IpdBeds_Create")]
    public async Task<ActionResult<BaseResponse<BedResponseDto>>> Create(
        [FromBody] CreateBedDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(OperationId = "IpdBeds_Update")]
    public async Task<ActionResult<BaseResponse<BedResponseDto>>> Update(
        long id,
        [FromBody] UpdateBedDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(OperationId = "IpdBeds_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        return Ok(await _service.DeleteAsync(id, cancellationToken));
    }
}
