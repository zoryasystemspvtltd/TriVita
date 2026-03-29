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
/// REST API for HMS Prescription notes.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/prescription-notes")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Prescription notes")]
public sealed class PrescriptionNotesController : ControllerBase
{
    private readonly IPrescriptionNoteService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PrescriptionNotesController> _logger;

    public PrescriptionNotesController(IPrescriptionNoteService service, ITenantContext tenant, ILogger<PrescriptionNotesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "PrescriptionNotes_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PrescriptionNoteResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<PrescriptionNoteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PrescriptionNoteResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PrescriptionNotes GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS PrescriptionNotes GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "PrescriptionNotes_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PrescriptionNoteResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<PrescriptionNoteResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<PrescriptionNoteResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? prescriptionId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS PrescriptionNotes GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, prescriptionId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "PrescriptionNotes_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PrescriptionNoteResponseDto>))]
    public async Task<ActionResult<BaseResponse<PrescriptionNoteResponseDto>>> Create([FromBody] CreatePrescriptionNoteDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "PrescriptionNotes_Update")]
    public async Task<ActionResult<BaseResponse<PrescriptionNoteResponseDto>>> Update(long id, [FromBody] UpdatePrescriptionNoteDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "PrescriptionNotes_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
