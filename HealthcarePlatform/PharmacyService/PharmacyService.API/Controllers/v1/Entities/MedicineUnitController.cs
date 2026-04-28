using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/medicine-unit")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrUnit")]
public sealed class MedicineUnitController : ControllerBase
{
    private readonly IPhrUnitService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<MedicineUnitController> _logger;

    public MedicineUnitController(IPhrUnitService service, ITenantContext tenant, ILogger<MedicineUnitController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "MedicineUnit_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<UnitResponseDto>))]
    public async Task<ActionResult<BaseResponse<UnitResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "MedicineUnit_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<UnitResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<UnitResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<UnitResponseDto>>> Create([FromBody] CreateUnitDto dto, CancellationToken ct)
    {
        var res = await _service.CreateAsync(dto, ct);
        if (res.Success) return Ok(res);
        if (res.Message == "Unit already exists with same name/code/symbol.") return Conflict(res);
        return BadRequest(res);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<UnitResponseDto>>> Update(long id, [FromBody] UpdateUnitDto dto, CancellationToken ct)
    {
        var res = await _service.UpdateAsync(id, dto, ct);
        if (res.Success) return Ok(res);
        if (res.Message == "Unit already exists with same name/code/symbol.") return Conflict(res);
        return BadRequest(res);
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
