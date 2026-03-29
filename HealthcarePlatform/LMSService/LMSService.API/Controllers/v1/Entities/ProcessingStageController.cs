using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/processing-stage")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LmsProcessingStage")]
public sealed class ProcessingStageController : ControllerBase
{
    private readonly ILmsProcessingStageService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ProcessingStageController> _logger;

    public ProcessingStageController(ILmsProcessingStageService service, ITenantContext tenant, ILogger<ProcessingStageController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ProcessingStage_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<ProcessingStageResponseDto>))]
    public async Task<ActionResult<BaseResponse<ProcessingStageResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ProcessingStage_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<ProcessingStageResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<ProcessingStageResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ProcessingStageResponseDto>>> Create([FromBody] CreateProcessingStageDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ProcessingStageResponseDto>>> Update(long id, [FromBody] UpdateProcessingStageDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
