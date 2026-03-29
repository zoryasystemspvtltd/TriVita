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
[Route("api/v{version:apiVersion}/qc-record")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LmsQcRecord")]
public sealed class QcRecordController : ControllerBase
{
    private readonly ILmsQcRecordService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<QcRecordController> _logger;

    public QcRecordController(ILmsQcRecordService service, ITenantContext tenant, ILogger<QcRecordController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "QcRecord_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<QcRecordResponseDto>))]
    public async Task<ActionResult<BaseResponse<QcRecordResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "QcRecord_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<QcRecordResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<QcRecordResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<QcRecordResponseDto>>> Create([FromBody] CreateQcRecordDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<QcRecordResponseDto>>> Update(long id, [FromBody] UpdateQcRecordDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
