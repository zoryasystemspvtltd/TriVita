using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using LMSService.Application.DTOs.Workflow;
using LMSService.Application.Services.Workflow;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Workflow;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/workflow/integration")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS workflow integration (LIS middleware)")]
public sealed class WorkflowIntegrationController : ControllerBase
{
    private readonly ILmsWorkflowIntegrationService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<WorkflowIntegrationController> _logger;

    public WorkflowIntegrationController(
        ILmsWorkflowIntegrationService service,
        ITenantContext tenant,
        ILogger<WorkflowIntegrationController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("barcode/{barcodeValue}")]
    [SwaggerOperation(Summary = "Resolve barcode to catalog test, parameters, and equipment assay codes", OperationId = "LmsWorkflow_ResolveBarcode")]
    public async Task<ActionResult<BaseResponse<LmsBarcodeResolutionDto>>> ResolveBarcode(
        string barcodeValue,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("LMS integration resolve barcode tenant {TenantId}", _tenant.TenantId);
        return Ok(await _service.ResolveBarcodeAsync(barcodeValue, cancellationToken));
    }
}
