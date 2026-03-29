using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.Application.DTOs.Entities;
using LISService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LISService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test-reference-range")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LisTestReferenceRange")]
public sealed class TestReferenceRangeController : ControllerBase
{
    private readonly ILisTestReferenceRangeService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<TestReferenceRangeController> _logger;

    public TestReferenceRangeController(ILisTestReferenceRangeService service, ITenantContext tenant, ILogger<TestReferenceRangeController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "TestReferenceRange_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<TestReferenceRangeResponseDto>))]
    public async Task<ActionResult<BaseResponse<TestReferenceRangeResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "TestReferenceRange_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<TestReferenceRangeResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<TestReferenceRangeResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<TestReferenceRangeResponseDto>>> Create([FromBody] CreateTestReferenceRangeDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<TestReferenceRangeResponseDto>>> Update(long id, [FromBody] UpdateTestReferenceRangeDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
