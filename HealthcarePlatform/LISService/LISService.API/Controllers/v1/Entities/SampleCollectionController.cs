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
[Route("api/v{version:apiVersion}/sample-collection")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LisSampleCollection")]
public sealed class SampleCollectionController : ControllerBase
{
    private readonly ILisSampleCollectionService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<SampleCollectionController> _logger;

    public SampleCollectionController(ILisSampleCollectionService service, ITenantContext tenant, ILogger<SampleCollectionController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "SampleCollection_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<SampleCollectionResponseDto>))]
    public async Task<ActionResult<BaseResponse<SampleCollectionResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "SampleCollection_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<SampleCollectionResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<SampleCollectionResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<SampleCollectionResponseDto>>> Create([FromBody] CreateSampleCollectionDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<SampleCollectionResponseDto>>> Update(long id, [FromBody] UpdateSampleCollectionDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
