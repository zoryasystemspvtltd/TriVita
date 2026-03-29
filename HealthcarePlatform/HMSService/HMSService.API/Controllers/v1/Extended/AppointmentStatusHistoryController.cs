using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Extended;
using HMSService.Application.Services.Extended;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1.Extended;

/// <summary>
/// REST API for HMS Appointment status history.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/appointment-status-history")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Appointment status history")]
public sealed class AppointmentStatusHistoryController : ControllerBase
{
    private readonly IAppointmentStatusHistoryService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<AppointmentStatusHistoryController> _logger;

    public AppointmentStatusHistoryController(IAppointmentStatusHistoryService service, ITenantContext tenant, ILogger<AppointmentStatusHistoryController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "AppointmentStatusHistory_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<AppointmentStatusHistoryResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<AppointmentStatusHistoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<AppointmentStatusHistoryResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS AppointmentStatusHistory GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS AppointmentStatusHistory GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "AppointmentStatusHistory_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<AppointmentStatusHistoryResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<AppointmentStatusHistoryResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<AppointmentStatusHistoryResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? appointmentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS AppointmentStatusHistory GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, appointmentId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "AppointmentStatusHistory_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<AppointmentStatusHistoryResponseDto>))]
    public async Task<ActionResult<BaseResponse<AppointmentStatusHistoryResponseDto>>> Create([FromBody] CreateAppointmentStatusHistoryDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "AppointmentStatusHistory_Update")]
    public async Task<ActionResult<BaseResponse<AppointmentStatusHistoryResponseDto>>> Update(long id, [FromBody] UpdateAppointmentStatusHistoryDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "AppointmentStatusHistory_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
