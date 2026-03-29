using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lab-inventory-transaction")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LmsLabInventoryTransaction")]
public sealed class LabInventoryTransactionController : ControllerBase
{
    private readonly ILmsLabInventoryTransactionService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabInventoryTransactionController> _logger;

    public LabInventoryTransactionController(ILmsLabInventoryTransactionService service, ITenantContext tenant, ILogger<LabInventoryTransactionController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "LabInventoryTransaction_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<LabInventoryTransactionResponseDto>))]
    public async Task<ActionResult<BaseResponse<LabInventoryTransactionResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "LabInventoryTransaction_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<LabInventoryTransactionResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<LabInventoryTransactionResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LabInventoryTransactionResponseDto>>> Create([FromBody] CreateLabInventoryTransactionDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LabInventoryTransactionResponseDto>>> Update(long id, [FromBody] UpdateLabInventoryTransactionDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
