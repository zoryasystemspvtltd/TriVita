using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/stock-ledger")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrStockLedger")]
public sealed class StockLedgerController : ControllerBase
{
    private readonly IPhrStockLedgerService _service;
    private readonly IStockLedgerReportingService _reports;
    private readonly ITenantContext _tenant;
    private readonly ILogger<StockLedgerController> _logger;

    public StockLedgerController(
        IPhrStockLedgerService service,
        IStockLedgerReportingService reports,
        ITenantContext tenant,
        ILogger<StockLedgerController> logger)
    {
        _service = service;
        _reports = reports;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "StockLedger_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<StockLedgerResponseDto>))]
    public async Task<ActionResult<BaseResponse<StockLedgerResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "StockLedger_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<StockLedgerResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<StockLedgerResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpGet("detailed")]
    [SwaggerOperation(OperationId = "StockLedger_GetDetailedReport")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<StockLedgerDetailedRowDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<StockLedgerDetailedRowDto>>>> GetDetailed(
        [FromQuery] StockLedgerReportQuery query,
        CancellationToken ct)
        => Ok(await _reports.GetDetailedAsync(query, ct));

    [HttpGet("summary")]
    [SwaggerOperation(OperationId = "StockLedger_GetSummaryReport")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<StockLedgerSummaryReportRowDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<StockLedgerSummaryReportRowDto>>>> GetSummary(
        [FromQuery] StockLedgerReportQuery query,
        CancellationToken ct)
        => Ok(await _reports.GetSummaryAsync(query, ct));

    [HttpGet("detailed/export")]
    [SwaggerOperation(OperationId = "StockLedger_ExportDetailed")]
    public async Task<IActionResult> ExportDetailed([FromQuery] StockLedgerReportQuery query, CancellationToken ct)
    {
        var res = await _reports.ExportDetailedExcelAsync(query, ct);
        if (!res.Success || res.Data is null)
            return BadRequest(res);
        var name = $"StockLedger_Detailed_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(res.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", name);
    }

    [HttpGet("summary/export")]
    [SwaggerOperation(OperationId = "StockLedger_ExportSummary")]
    public async Task<IActionResult> ExportSummary([FromQuery] StockLedgerReportQuery query, CancellationToken ct)
    {
        var res = await _reports.ExportSummaryExcelAsync(query, ct);
        if (!res.Success || res.Data is null)
            return BadRequest(res);
        var name = $"StockLedger_Summary_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
        return File(res.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", name);
    }
}
