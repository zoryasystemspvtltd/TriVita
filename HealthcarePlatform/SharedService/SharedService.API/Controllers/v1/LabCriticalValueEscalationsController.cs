using Asp.Versioning;
using Healthcare.Common.Authorization;
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
[Route("api/v{version:apiVersion}/lab-critical-value-escalations")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("09 — Lab critical value escalation (EXT_ table)")]
public sealed class LabCriticalValueEscalationsController : ControllerBase
{
    private readonly ILabCriticalValueEscalationService _service;

    public LabCriticalValueEscalationsController(ILabCriticalValueEscalationService service)
    {
        _service = service;
    }

    [HttpGet("by-lab-result")]
    [SwaggerOperation(Summary = "List escalations for a lab result", OperationId = "Shared_LabCritical_ListByResult")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<LabCriticalValueEscalationResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<LabCriticalValueEscalationResponseDto>>>> ListByLabResult(
        [FromQuery] long facilityId,
        [FromQuery] long labResultId,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListByLabResultAsync(facilityId, labResultId, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get escalation by id", OperationId = "Shared_LabCritical_GetById")]
    [ProducesResponseType(typeof(BaseResponse<LabCriticalValueEscalationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<LabCriticalValueEscalationResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create escalation", OperationId = "Shared_LabCritical_Create")]
    [ProducesResponseType(typeof(BaseResponse<LabCriticalValueEscalationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<LabCriticalValueEscalationResponseDto>>> Create(
        [FromBody] CreateLabCriticalValueEscalationDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update escalation (dispatch / acknowledge)", OperationId = "Shared_LabCritical_Update")]
    [ProducesResponseType(typeof(BaseResponse<LabCriticalValueEscalationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<LabCriticalValueEscalationResponseDto>>> Update(
        long id,
        [FromBody] UpdateLabCriticalValueEscalationDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete escalation", OperationId = "Shared_LabCritical_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
