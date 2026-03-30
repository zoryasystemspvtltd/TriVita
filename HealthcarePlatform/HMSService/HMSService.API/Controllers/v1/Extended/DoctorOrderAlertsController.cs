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
[Route("api/v{version:apiVersion}/doctor-order-alerts")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Doctor order alerts")]
public sealed class DoctorOrderAlertsController : ControllerBase
{
    private readonly IDoctorOrderAlertService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<DoctorOrderAlertsController> _logger;

    public DoctorOrderAlertsController(
        IDoctorOrderAlertService service,
        ITenantContext tenant,
        ILogger<DoctorOrderAlertsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "DoctorOrderAlerts_GetById")]
    public async Task<ActionResult<BaseResponse<DoctorOrderAlertResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS DoctorOrderAlert GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "DoctorOrderAlerts_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<DoctorOrderAlertResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? visitId,
        [FromQuery] long? admissionId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, visitId, admissionId, cancellationToken));
    }

    [HttpPost]
    [SwaggerOperation(OperationId = "DoctorOrderAlerts_Create")]
    public async Task<ActionResult<BaseResponse<DoctorOrderAlertResponseDto>>> Create(
        [FromBody] CreateDoctorOrderAlertDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPost("{id:long}/acknowledge")]
    [SwaggerOperation(OperationId = "DoctorOrderAlerts_Acknowledge")]
    public async Task<ActionResult<BaseResponse<DoctorOrderAlertResponseDto>>> Acknowledge(
        long id,
        [FromBody] AcknowledgeDoctorOrderAlertDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.AcknowledgeAsync(id, dto, cancellationToken));
    }
}
