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
/// REST API for HMS Procedures.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/procedures")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS Procedures")]
public sealed class MedicalProceduresController : ControllerBase
{
    private readonly IMedicalProcedureService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<MedicalProceduresController> _logger;

    public MedicalProceduresController(IMedicalProcedureService service, ITenantContext tenant, ILogger<MedicalProceduresController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Gets an entity by id.</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get by id", OperationId = "MedicalProcedures_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<MedicalProcedureResponseDto>))]
    [ProducesResponseType(typeof(BaseResponse<MedicalProcedureResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<MedicalProcedureResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS MedicalProcedures GetById id {Id} tenant {TenantId}", id, _tenant.TenantId);
        var result = await _service.GetByIdAsync(id, cancellationToken);
        _logger.LogInformation("HMS MedicalProcedures GetById success {Success}", result.Success);
        return Ok(result);
    }

    /// <summary>Paged list with optional filters.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "List paged", OperationId = "MedicalProcedures_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<MedicalProcedureResponseDto>>))]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<MedicalProcedureResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<MedicalProcedureResponseDto>>>> GetPaged([FromQuery] PagedQuery query, [FromQuery] long? visitId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HMS MedicalProcedures GetPaged tenant {TenantId}", _tenant.TenantId);
        var result = await _service.GetPagedAsync(query, visitId, cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new record.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create", OperationId = "MedicalProcedures_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<MedicalProcedureResponseDto>))]
    public async Task<ActionResult<BaseResponse<MedicalProcedureResponseDto>>> Create([FromBody] CreateMedicalProcedureDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates an existing record.</summary>
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update", OperationId = "MedicalProcedures_Update")]
    public async Task<ActionResult<BaseResponse<MedicalProcedureResponseDto>>> Update(long id, [FromBody] UpdateMedicalProcedureDto dto, CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Soft-deletes a record.</summary>
    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "MedicalProcedures_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return Ok(result);
    }
}
