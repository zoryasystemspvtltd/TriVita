using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using LMSService.Application.DTOs.Workflow;
using LMSService.Application.Services.Workflow;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Workflow;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/workflow/equipment-types")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS equipment type master")]
public sealed class LabWorkflowEquipmentTypesController : ControllerBase
{
    private readonly ILmsEquipmentTypeService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabWorkflowEquipmentTypesController> _logger;

    public LabWorkflowEquipmentTypesController(
        ILmsEquipmentTypeService service,
        ITenantContext tenant,
        ILogger<LabWorkflowEquipmentTypesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "LmsEquipmentType_GetById")]
    public async Task<ActionResult<BaseResponse<LmsEquipmentTypeResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("LMS EquipmentType GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "LmsEquipmentType_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsEquipmentTypeResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "LmsEquipmentType_Create")]
    public async Task<ActionResult<BaseResponse<LmsEquipmentTypeResponseDto>>> Create(
        [FromBody] CreateLmsEquipmentTypeDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(OperationId = "LmsEquipmentType_Update")]
    public async Task<ActionResult<BaseResponse<LmsEquipmentTypeResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsEquipmentTypeDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(OperationId = "LmsEquipmentType_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        return Ok(await _service.DeleteAsync(id, cancellationToken));
    }
}
