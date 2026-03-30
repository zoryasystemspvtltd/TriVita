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
[Route("api/v{version:apiVersion}/emar-entries")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS eMAR")]
public sealed class EmarEntriesController : ControllerBase
{
    private readonly IEmarEntryService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<EmarEntriesController> _logger;

    public EmarEntriesController(IEmarEntryService service, ITenantContext tenant, ILogger<EmarEntriesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "Emar_GetById")]
    public async Task<ActionResult<BaseResponse<EmarEntryResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS eMAR GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "Emar_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<EmarEntryResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? admissionId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, admissionId, cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "Emar_Create")]
    public async Task<ActionResult<BaseResponse<EmarEntryResponseDto>>> Create(
        [FromBody] CreateEmarEntryDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(OperationId = "Emar_Update")]
    public async Task<ActionResult<BaseResponse<EmarEntryResponseDto>>> Update(
        long id,
        [FromBody] UpdateEmarEntryDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(OperationId = "Emar_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        return Ok(await _service.DeleteAsync(id, cancellationToken));
    }
}
