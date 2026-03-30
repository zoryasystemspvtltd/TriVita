using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using HMSService.Application.DTOs.Gap;
using HMSService.Application.Services.Gap;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1.Extended;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/patient-masters")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Patient Master / UPID")]
public sealed class PatientMastersController : ControllerBase
{
    private readonly IPatientMasterService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PatientMastersController> _logger;

    public PatientMastersController(
        IPatientMasterService service,
        ITenantContext tenant,
        ILogger<PatientMastersController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get patient master by id", OperationId = "PatientMasters_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PatientMasterResponseDto>))]
    public async Task<ActionResult<BaseResponse<PatientMasterResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PatientMaster GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Search patient masters (cross-facility when facility filter omitted)", OperationId = "PatientMasters_Search")]
    public async Task<ActionResult<BaseResponse<PagedResponse<PatientMasterResponseDto>>>> Search(
        [FromQuery] PagedQuery query,
        [FromQuery] string? search,
        [FromQuery] long? linkedFacilityId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.SearchPagedAsync(query, search, linkedFacilityId, cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create patient master (UPID generated)", OperationId = "PatientMasters_Create")]
    public async Task<ActionResult<BaseResponse<PatientMasterResponseDto>>> Create(
        [FromBody] CreatePatientMasterDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update patient master", OperationId = "PatientMasters_Update")]
    public async Task<ActionResult<BaseResponse<PatientMasterResponseDto>>> Update(
        long id,
        [FromBody] UpdatePatientMasterDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpPost("link-facility")]
    [SwaggerOperation(Summary = "Link patient master to a facility", OperationId = "PatientMasters_LinkFacility")]
    public async Task<ActionResult<BaseResponse<PatientMasterResponseDto>>> LinkFacility(
        [FromBody] LinkPatientFacilityDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.LinkFacilityAsync(dto, cancellationToken));
    }
}
