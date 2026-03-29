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
/// REST API for HMS Appointment queue.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/appointment-queue")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Appointment queue")]
public sealed class AppointmentQueueController : ControllerBase
{
    private readonly IAppointmentQueueService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<AppointmentQueueController> _logger;

    public AppointmentQueueController(IAppointmentQueueService service, ITenantContext tenant, ILogger<AppointmentQueueController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "AppointmentQueue_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<AppointmentQueueResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<AppointmentQueueResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<AppointmentQueueResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS AppointmentQueue GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS AppointmentQueue GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "AppointmentQueue_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<AppointmentQueueResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<AppointmentQueueResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<AppointmentQueueResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? appointmentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS AppointmentQueue GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, appointmentId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "AppointmentQueue_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<AppointmentQueueResponseDto>))]
    public async Task<ActionResult<BaseResponse<AppointmentQueueResponseDto>>> Create([FromBody] CreateAppointmentQueueDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "AppointmentQueue_Update")]
    public async Task<ActionResult<BaseResponse<AppointmentQueueResponseDto>>> Update(long id, [FromBody] UpdateAppointmentQueueDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "AppointmentQueue_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
