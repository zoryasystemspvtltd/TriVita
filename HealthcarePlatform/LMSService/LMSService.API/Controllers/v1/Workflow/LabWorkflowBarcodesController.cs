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
[Route("api/v{version:apiVersion}/workflow/barcodes")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS sample barcode (LMS-owned)")]
public sealed class LabWorkflowBarcodesController : ControllerBase
{
    private readonly ILmsLabSampleBarcodeService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabWorkflowBarcodesController> _logger;

    public LabWorkflowBarcodesController(
        ILmsLabSampleBarcodeService service,
        ITenantContext tenant,
        ILogger<LabWorkflowBarcodesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get barcode registration by id", OperationId = "LmsBarcode_GetById")]
    public async Task<ActionResult<BaseResponse<LmsLabSampleBarcodeResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("LMS Barcode GetById {Id}", id);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register barcode (external collection system)", OperationId = "LmsBarcode_Register")]
    public async Task<ActionResult<BaseResponse<LmsLabSampleBarcodeResponseDto>>> Register(
        [FromBody] RegisterLmsLabSampleBarcodeDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.RegisterAsync(dto, cancellationToken));
    }
}
