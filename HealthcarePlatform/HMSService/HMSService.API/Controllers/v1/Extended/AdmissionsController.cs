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
[Route("api/v{version:apiVersion}/admissions")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS IPD Admissions")]
public sealed class AdmissionsController : ControllerBase
{
    private readonly IAdmissionWorkflowService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<AdmissionsController> _logger;

    public AdmissionsController(
        IAdmissionWorkflowService service,
        ITenantContext tenant,
        ILogger<AdmissionsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get admission by id", OperationId = "Admissions_GetById")]
    public async Task<ActionResult<BaseResponse<AdmissionResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS Admission GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List admissions paged", OperationId = "Admissions_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<AdmissionResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? patientMasterId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, patientMasterId, cancellationToken));
    }

    [HttpPost("admit")]
    [SwaggerOperation(Summary = "Admit patient to bed", OperationId = "Admissions_Admit")]
    public async Task<ActionResult<BaseResponse<AdmissionResponseDto>>> Admit(
        [FromBody] AdmitPatientDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.AdmitAsync(dto, cancellationToken));
    }

    [HttpPost("transfer")]
    [SwaggerOperation(Summary = "Transfer admission to another bed", OperationId = "Admissions_Transfer")]
    public async Task<ActionResult<BaseResponse<AdmissionResponseDto>>> Transfer(
        [FromBody] TransferPatientDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.TransferAsync(dto, cancellationToken));
    }

    [HttpPost("discharge")]
    [SwaggerOperation(Summary = "Discharge admission", OperationId = "Admissions_Discharge")]
    public async Task<ActionResult<BaseResponse<AdmissionResponseDto>>> Discharge(
        [FromBody] DischargePatientDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.DischargeAsync(dto, cancellationToken));
    }
}
