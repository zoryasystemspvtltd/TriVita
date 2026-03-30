using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using LISService.Application.DTOs.Analyzer;
using LISService.Application.Services.Analyzer;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LISService.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/analyzer")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LIS analyzer middleware (uses LMS for master resolution)")]
public sealed class AnalyzerIntegrationController : ControllerBase
{
    private readonly ILisAnalyzerIntegrationService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<AnalyzerIntegrationController> _logger;

    public AnalyzerIntegrationController(
        ILisAnalyzerIntegrationService service,
        ITenantContext tenant,
        ILogger<AnalyzerIntegrationController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("query-test")]
    [SwaggerOperation(Summary = "Resolve barcode via LMS; return equipment-specific test codes", OperationId = "LisAnalyzer_QueryTest")]
    public async Task<ActionResult<BaseResponse<AnalyzerQueryTestResponseDto>>> QueryTest(
        [FromQuery] string barcode,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("LIS analyzer query-test tenant {TenantId}", _tenant.TenantId);
        return Ok(await _service.QueryTestAsync(barcode, cancellationToken));
    }

    [HttpPost("result")]
    [SwaggerOperation(Summary = "Ingest analyzer results (mapped via LMS parameters)", OperationId = "LisAnalyzer_IngestResult")]
    public async Task<ActionResult<BaseResponse<AnalyzerResultIngestResponseDto>>> IngestResult(
        [FromBody] AnalyzerResultIngestDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.IngestResultAsync(dto, cancellationToken));
    }
}
