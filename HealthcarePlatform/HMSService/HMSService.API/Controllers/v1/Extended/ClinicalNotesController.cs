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
/// REST API for HMS Clinical notes.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/clinical-notes")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Clinical notes")]
public sealed class ClinicalNotesController : ControllerBase
{
    private readonly IClinicalNoteService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ClinicalNotesController> _logger;

    public ClinicalNotesController(IClinicalNoteService service, ITenantContext tenant, ILogger<ClinicalNotesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "ClinicalNotes_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<ClinicalNoteResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<ClinicalNoteResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<ClinicalNoteResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS ClinicalNotes GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS ClinicalNotes GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "ClinicalNotes_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<ClinicalNoteResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<ClinicalNoteResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<ClinicalNoteResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? visitId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS ClinicalNotes GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, visitId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "ClinicalNotes_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<ClinicalNoteResponseDto>))]
    public async Task<ActionResult<BaseResponse<ClinicalNoteResponseDto>>> Create([FromBody] CreateClinicalNoteDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "ClinicalNotes_Update")]
    public async Task<ActionResult<BaseResponse<ClinicalNoteResponseDto>>> Update(long id, [FromBody] UpdateClinicalNoteDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "ClinicalNotes_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
