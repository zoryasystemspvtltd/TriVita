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
[Route("api/v{version:apiVersion}/workflow/test-masters")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS catalog test master (LIS consumes via integration API)")]
public sealed class LabWorkflowTestMastersController : ControllerBase
{
    private readonly ILmsCatalogTestService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabWorkflowTestMastersController> _logger;

    public LabWorkflowTestMastersController(
        ILmsCatalogTestService service,
        ITenantContext tenant,
        ILogger<LabWorkflowTestMastersController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsCatalogTestResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("LMS CatalogTest GetById {Id}", id);
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsCatalogTestResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetPagedAsync(query, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsCatalogTestResponseDto>>> Create(
        [FromBody] CreateLmsCatalogTestDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.CreateAsync(dto, cancellationToken));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsCatalogTestResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsCatalogTestDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        return Ok(await _service.DeleteAsync(id, cancellationToken));
    }
}
