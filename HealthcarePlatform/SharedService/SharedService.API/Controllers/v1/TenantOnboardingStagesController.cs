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
[Route("api/v{version:apiVersion}/tenant-onboarding-stages")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("09 — Tenant onboarding stages")]
public sealed class TenantOnboardingStagesController : ControllerBase
{
    private readonly ITenantOnboardingStageService _service;

    public TenantOnboardingStagesController(ITenantOnboardingStageService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List onboarding stages for tenant", OperationId = "Shared_Onboarding_List")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<TenantOnboardingStageResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<TenantOnboardingStageResponseDto>>>> List(CancellationToken cancellationToken) =>
        Ok(await _service.ListAsync(cancellationToken));

    [HttpGet("by-code")]
    [SwaggerOperation(Summary = "Get stage by code", OperationId = "Shared_Onboarding_GetByCode")]
    [ProducesResponseType(typeof(BaseResponse<TenantOnboardingStageResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<TenantOnboardingStageResponseDto>>> GetByCode(
        [FromQuery] string stageCode,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByStageCodeAsync(stageCode, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get stage by id", OperationId = "Shared_Onboarding_GetById")]
    [ProducesResponseType(typeof(BaseResponse<TenantOnboardingStageResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<TenantOnboardingStageResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("upsert")]
    [SwaggerOperation(Summary = "Create or update stage by stage code", OperationId = "Shared_Onboarding_Upsert")]
    [ProducesResponseType(typeof(BaseResponse<TenantOnboardingStageResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<TenantOnboardingStageResponseDto>>> Upsert(
        [FromBody] UpsertTenantOnboardingStageDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpsertAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete stage", OperationId = "Shared_Onboarding_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
