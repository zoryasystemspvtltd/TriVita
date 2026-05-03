using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ITenantContext _tenant;
    private readonly ILogger<StockLedgerController> _logger;

    public StockLedgerController(IPhrStockLedgerService service, ITenantContext tenant, ILogger<StockLedgerController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

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

    [HttpGet("report/detail")]
    [SwaggerOperation(OperationId = "StockLedger_GetReportDetailPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<StockLedgerResponseDto>>>> GetReportDetailPaged(
        [FromQuery] StockLedgerReportQuery query,
        CancellationToken ct)
        => Ok(await _service.GetReportPagedAsync(query, ct));

    [HttpGet("report/summary")]
    [SwaggerOperation(OperationId = "StockLedger_GetSummaryReport")]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<StockLedgerSummaryRowDto>>>> GetSummaryReport(
        [FromQuery] StockLedgerReportQuery filter,
        CancellationToken ct)
        => Ok(await _service.GetSummaryReportAsync(filter, ct));
}
