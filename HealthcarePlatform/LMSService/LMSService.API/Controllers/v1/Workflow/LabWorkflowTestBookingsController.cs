using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using LMSService.Application.DTOs.Workflow;
using LMSService.Application.Services.Workflow;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Workflow;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/workflow/test-bookings")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS lab test booking (OPD/IPD)")]
public sealed class LabWorkflowTestBookingsController : ControllerBase
{
    private readonly ILmsLabTestBookingService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabWorkflowTestBookingsController> _logger;

    public LabWorkflowTestBookingsController(
        ILmsLabTestBookingService service,
        ITenantContext tenant,
        ILogger<LabWorkflowTestBookingsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsLabTestBookingResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("LMS LabTestBooking GetById {Id}", id);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsLabTestBookingResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsLabTestBookingResponseDto>>> Create(
        [FromBody] CreateLmsLabTestBookingDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }
}
