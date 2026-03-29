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
[Route("api/v{version:apiVersion}/equipment-maintenance")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LmsEquipmentMaintenance")]
public sealed class EquipmentMaintenanceController : ControllerBase
{
    private readonly ILmsEquipmentMaintenanceService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<EquipmentMaintenanceController> _logger;

    public EquipmentMaintenanceController(ILmsEquipmentMaintenanceService service, ITenantContext tenant, ILogger<EquipmentMaintenanceController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "EquipmentMaintenance_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<EquipmentMaintenanceResponseDto>))]
    public async Task<ActionResult<BaseResponse<EquipmentMaintenanceResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "EquipmentMaintenance_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<EquipmentMaintenanceResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<EquipmentMaintenanceResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<EquipmentMaintenanceResponseDto>>> Create([FromBody] CreateEquipmentMaintenanceDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<EquipmentMaintenanceResponseDto>>> Update(long id, [FromBody] UpdateEquipmentMaintenanceDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
