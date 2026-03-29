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
[Route("api/v{version:apiVersion}/module-integration-handoffs")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("09 — Module integration handoff log")]
public sealed class ModuleIntegrationHandoffsController : ControllerBase
{
    private readonly IModuleIntegrationHandoffService _service;

    public ModuleIntegrationHandoffsController(IModuleIntegrationHandoffService service)
    {
        _service = service;
    }

    [HttpGet("by-correlation")]
    [SwaggerOperation(Summary = "List handoffs by correlation id", OperationId = "Shared_Handoffs_ByCorrelation")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<ModuleIntegrationHandoffResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<ModuleIntegrationHandoffResponseDto>>>> ListByCorrelation(
        [FromQuery] string correlationId,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListByCorrelationAsync(correlationId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get handoff by id", OperationId = "Shared_Handoffs_GetById")]
    [ProducesResponseType(typeof(BaseResponse<ModuleIntegrationHandoffResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<ModuleIntegrationHandoffResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create handoff", OperationId = "Shared_Handoffs_Create")]
    [ProducesResponseType(typeof(BaseResponse<ModuleIntegrationHandoffResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<ModuleIntegrationHandoffResponseDto>>> Create(
        [FromBody] CreateModuleIntegrationHandoffDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update handoff", OperationId = "Shared_Handoffs_Update")]
    [ProducesResponseType(typeof(BaseResponse<ModuleIntegrationHandoffResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<ModuleIntegrationHandoffResponseDto>>> Update(
        long id,
        [FromBody] UpdateModuleIntegrationHandoffDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete handoff", OperationId = "Shared_Handoffs_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
