using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using Swashbuckle.AspNetCore.Annotations;

namespace SharedService.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/enterprise-hierarchy")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("Enterprise hierarchy")]
public sealed class EnterpriseHierarchyController : ControllerBase
{
    private readonly IEnterpriseService _enterpriseService;

    public EnterpriseHierarchyController(IEnterpriseService enterpriseService)
    {
        _enterpriseService = enterpriseService;
    }

    /// <summary>Returns the full enterprise tree: companies, business units, facilities, and departments.</summary>
    [HttpGet("{enterpriseId:long}")]
    [SwaggerOperation(Summary = "Get full hierarchy for an enterprise", OperationId = "Shared_EnterpriseHierarchy_Get")]
    [ProducesResponseType(typeof(BaseResponse<EnterpriseHierarchyResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<EnterpriseHierarchyResponseDto>>> GetTree(
        long enterpriseId,
        CancellationToken cancellationToken)
    {
        var result = await _enterpriseService.GetHierarchyAsync(enterpriseId, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
