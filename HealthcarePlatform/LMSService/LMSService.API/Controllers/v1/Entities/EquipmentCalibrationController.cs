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
[Route("api/v{version:apiVersion}/equipment-calibration")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LmsEquipmentCalibration")]
public sealed class EquipmentCalibrationController : ControllerBase
{
    private readonly ILmsEquipmentCalibrationService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<EquipmentCalibrationController> _logger;

    public EquipmentCalibrationController(ILmsEquipmentCalibrationService service, ITenantContext tenant, ILogger<EquipmentCalibrationController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "EquipmentCalibration_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<EquipmentCalibrationResponseDto>))]
    public async Task<ActionResult<BaseResponse<EquipmentCalibrationResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "EquipmentCalibration_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<EquipmentCalibrationResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<EquipmentCalibrationResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<EquipmentCalibrationResponseDto>>> Create([FromBody] CreateEquipmentCalibrationDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<EquipmentCalibrationResponseDto>>> Update(long id, [FromBody] UpdateEquipmentCalibrationDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
