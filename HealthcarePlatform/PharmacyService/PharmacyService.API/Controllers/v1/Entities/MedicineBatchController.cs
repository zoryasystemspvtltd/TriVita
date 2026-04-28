using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/medicine-batch")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrMedicineBatch")]
public sealed class MedicineBatchController : ControllerBase
{
    private readonly IPhrMedicineBatchService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<MedicineBatchController> _logger;

    public MedicineBatchController(IPhrMedicineBatchService service, ITenantContext tenant, ILogger<MedicineBatchController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "MedicineBatch_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<MedicineBatchResponseDto>))]
    public async Task<ActionResult<BaseResponse<MedicineBatchResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "MedicineBatch_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<MedicineBatchResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<MedicineBatchResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<MedicineBatchResponseDto>>> Create([FromBody] CreateMedicineBatchDto dto, CancellationToken ct)
    {
        var res = await _service.CreateAsync(dto, ct);
        if (res.Success) return Ok(res);
        if (res.Message == "Batch number already exists for the selected medicine.") return Conflict(res);
        return BadRequest(res);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<MedicineBatchResponseDto>>> Update(long id, [FromBody] UpdateMedicineBatchDto dto, CancellationToken ct)
    {
        var res = await _service.UpdateAsync(id, dto, ct);
        if (res.Success) return Ok(res);
        if (res.Message == "Batch number already exists for the selected medicine.") return Conflict(res);
        return BadRequest(res);
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
