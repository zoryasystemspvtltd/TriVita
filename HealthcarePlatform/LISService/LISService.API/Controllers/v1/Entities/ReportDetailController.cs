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
[Route("api/v{version:apiVersion}/report-detail")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LisReportDetail")]
public sealed class ReportDetailController : ControllerBase
{
    private readonly ILisReportDetailService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReportDetailController> _logger;

    public ReportDetailController(ILisReportDetailService service, ITenantContext tenant, ILogger<ReportDetailController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReportDetail_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<ReportDetailResponseDto>))]
    public async Task<ActionResult<BaseResponse<ReportDetailResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReportDetail_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<ReportDetailResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReportDetailResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReportDetailResponseDto>>> Create([FromBody] CreateReportDetailDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReportDetailResponseDto>>> Update(long id, [FromBody] UpdateReportDetailDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
