using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using PharmacyService.Application.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/integration/lms-inventory")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("Pharmacy â€” LMS / batch consumption integration")]
public sealed class LmsInventoryIntegrationController : ControllerBase
{
    private readonly ILmsInventoryIntegrationService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LmsInventoryIntegrationController> _logger;

    public LmsInventoryIntegrationController(
        ILmsInventoryIntegrationService service,
        ITenantContext tenant,
        ILogger<LmsInventoryIntegrationController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Webhook-style endpoint for LMS reagent consumption; logs with TenantId for audit.</summary>
    [HttpPost("reagent-consumption")]
    [SwaggerOperation(OperationId = "LmsInventory_RecordReagentConsumption")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> RecordReagentConsumption(
        [FromBody] RecordLmsReagentConsumptionRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "LmsInventoryIntegration POST tenant {TenantId} facility {FacilityId}",
            _tenant.TenantId,
            _tenant.FacilityId);

        return Ok(await _service.RecordLmsReagentConsumptionAsync(request, ct));
    }
}
