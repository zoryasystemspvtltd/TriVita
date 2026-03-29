using Asp.Versioning;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Appointments;
using HMSService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1;

/// <summary>
/// OPD appointments: create, list (paged), update, soft-delete.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/appointments")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("Appointments")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>Gets a single appointment by identifier.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get appointment by id", OperationId = "Appointments_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Wrapped result (check Success flag for not-found).", typeof(BaseResponse<AppointmentResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<AppointmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<AppointmentResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>Lists appointments with pagination, optional filters, and sorting.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List appointments (paged)", OperationId = "Appointments_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paged list wrapped in BaseResponse.", typeof(BaseResponse<PagedResponse<AppointmentResponseDto>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<AppointmentResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<PagedResponse<AppointmentResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? patientId,
        [FromQuery] long? doctorId,
        [FromQuery] DateTime? scheduledFrom,
        [FromQuery] DateTime? scheduledTo,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.GetPagedAsync(
            query,
            patientId,
            doctorId,
            scheduledFrom,
            scheduledTo,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates an appointment (requires facility context).</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create appointment", OperationId = "Appointments_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "Created or validation/business error in body.", typeof(BaseResponse<AppointmentResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<AppointmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<AppointmentResponseDto>>> Create(
        [FromBody] CreateAppointmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing appointment.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update appointment", OperationId = "Appointments_Update")]
    [SwaggerResponse(StatusCodes.Status200OK, "Updated or error in body.", typeof(BaseResponse<AppointmentResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<AppointmentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<AppointmentResponseDto>>> Update(
        long id,
        [FromBody] UpdateAppointmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _appointmentService.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes an appointment.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete appointment (soft)", OperationId = "Appointments_Delete")]
    [SwaggerResponse(StatusCodes.Status200OK, "Deleted or error in body.", typeof(BaseResponse<object>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _appointmentService.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
