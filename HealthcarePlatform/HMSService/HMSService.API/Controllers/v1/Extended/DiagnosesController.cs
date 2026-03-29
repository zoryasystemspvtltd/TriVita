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
/// REST API for HMS Diagnoses.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/diagnoses")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Diagnoses")]
public sealed class DiagnosesController : ControllerBase
{
    private readonly IDiagnosisService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<DiagnosesController> _logger;

    public DiagnosesController(IDiagnosisService service, ITenantContext tenant, ILogger<DiagnosesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "Diagnoses_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<DiagnosisResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<DiagnosisResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<DiagnosisResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS Diagnoses GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS Diagnoses GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "Diagnoses_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<DiagnosisResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<DiagnosisResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<DiagnosisResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? visitId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS Diagnoses GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, visitId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "Diagnoses_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<DiagnosisResponseDto>))]
    public async Task<ActionResult<BaseResponse<DiagnosisResponseDto>>> Create([FromBody] CreateDiagnosisDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "Diagnoses_Update")]
    public async Task<ActionResult<BaseResponse<DiagnosisResponseDto>>> Update(long id, [FromBody] UpdateDiagnosisDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "Diagnoses_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
