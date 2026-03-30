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
[Route("api/v{version:apiVersion}/ipd/wards")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS IPD Wards")]
public sealed class IpdWardsController : ControllerBase
{
    private readonly IWardService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IpdWardsController> _logger;

    public IpdWardsController(IWardService service, ITenantContext tenant, ILogger<IpdWardsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IpdWards_GetById")]
    public async Task<ActionResult<BaseResponse<WardResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS Ward GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IpdWards_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<WardResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "IpdWards_Create")]
    public async Task<ActionResult<BaseResponse<WardResponseDto>>> Create(
        [FromBody] CreateWardDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(OperationId = "IpdWards_Update")]
    public async Task<ActionResult<BaseResponse<WardResponseDto>>> Update(
        long id,
        [FromBody] UpdateWardDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(OperationId = "IpdWards_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        return Ok(await _service.DeleteAsync(id, cancellationToken));
    }
}
