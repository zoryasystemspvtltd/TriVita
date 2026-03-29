using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Application.Services.FeatureExtensions;
using Swashbuckle.AspNetCore.Annotations;

namespace SharedService.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cross-facility-report-audits")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("09 — Cross-facility report audit")]
public sealed class CrossFacilityReportAuditsController : ControllerBase
{
    private readonly ICrossFacilityReportAuditService _service;

    public CrossFacilityReportAuditsController(ICrossFacilityReportAuditService service)
    {
        _service = service;
    }

    [HttpGet("paged")]
    [SwaggerOperation(Summary = "List report audits (paged)", OperationId = "Shared_ReportAudits_GetPaged")]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<CrossFacilityReportAuditResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<CrossFacilityReportAuditResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] string? reportCode,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPagedAsync(query, reportCode, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get audit by id", OperationId = "Shared_ReportAudits_GetById")]
    [ProducesResponseType(typeof(BaseResponse<CrossFacilityReportAuditResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<CrossFacilityReportAuditResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create audit row", OperationId = "Shared_ReportAudits_Create")]
    [ProducesResponseType(typeof(BaseResponse<CrossFacilityReportAuditResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<CrossFacilityReportAuditResponseDto>>> Create(
        [FromBody] CreateCrossFacilityReportAuditDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update audit (completion, counts)", OperationId = "Shared_ReportAudits_Update")]
    [ProducesResponseType(typeof(BaseResponse<CrossFacilityReportAuditResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<CrossFacilityReportAuditResponseDto>>> Update(
        long id,
        [FromBody] UpdateCrossFacilityReportAuditDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete audit", OperationId = "Shared_ReportAudits_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
