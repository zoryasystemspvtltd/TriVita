using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.Application.DTOs.Entities;
using LISService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LISService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lab-order-item")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LisLabOrderItem")]
public sealed class LabOrderItemController : ControllerBase
{
    private readonly ILisLabOrderItemService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabOrderItemController> _logger;

    public LabOrderItemController(ILisLabOrderItemService service, ITenantContext tenant, ILogger<LabOrderItemController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "LabOrderItem_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<LabOrderItemResponseDto>))]
    public async Task<ActionResult<BaseResponse<LabOrderItemResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "LabOrderItem_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<LabOrderItemResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<LabOrderItemResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LabOrderItemResponseDto>>> Create([FromBody] CreateLabOrderItemDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LabOrderItemResponseDto>>> Update(long id, [FromBody] UpdateLabOrderItemDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
