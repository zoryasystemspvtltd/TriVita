using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/technician-assignment")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LmsTechnicianAssignment")]
public sealed class TechnicianAssignmentController : ControllerBase
{
    private readonly ILmsTechnicianAssignmentService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<TechnicianAssignmentController> _logger;

    public TechnicianAssignmentController(ILmsTechnicianAssignmentService service, ITenantContext tenant, ILogger<TechnicianAssignmentController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "TechnicianAssignment_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<TechnicianAssignmentResponseDto>))]
    public async Task<ActionResult<BaseResponse<TechnicianAssignmentResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "TechnicianAssignment_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<TechnicianAssignmentResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<TechnicianAssignmentResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<TechnicianAssignmentResponseDto>>> Create([FromBody] CreateTechnicianAssignmentDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<TechnicianAssignmentResponseDto>>> Update(long id, [FromBody] UpdateTechnicianAssignmentDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
