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
[Route("api/v{version:apiVersion}/workflow/equipment-facility-mappings")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS equipment ↔ facility mapping")]
public sealed class LabWorkflowEquipmentFacilityMappingsController : ControllerBase
{
    private readonly ILmsEquipmentFacilityMappingService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabWorkflowEquipmentFacilityMappingsController> _logger;

    public LabWorkflowEquipmentFacilityMappingsController(
        ILmsEquipmentFacilityMappingService service,
        ITenantContext tenant,
        ILogger<LabWorkflowEquipmentFacilityMappingsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsEquipmentFacilityMappingResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("LMS EquipmentFacilityMapping GetById {Id}", id);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsEquipmentFacilityMappingResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsEquipmentFacilityMappingResponseDto>>> Create(
        [FromBody] CreateLmsEquipmentFacilityMappingDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsEquipmentFacilityMappingResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsEquipmentFacilityMappingDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        return Ok(await _service.DeleteAsync(id, cancellationToken));
    }
}
