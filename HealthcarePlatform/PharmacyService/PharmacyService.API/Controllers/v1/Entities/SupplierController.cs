using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/supplier")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrSupplier")]
public sealed class SupplierController : ControllerBase
{
    private readonly IPhrSupplierService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<SupplierController> _logger;

    public SupplierController(IPhrSupplierService service, ITenantContext tenant, ILogger<SupplierController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "Supplier_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<SupplierResponseDto>))]
    public async Task<ActionResult<BaseResponse<SupplierResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "Supplier_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<SupplierResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<SupplierResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<SupplierResponseDto>>> Create([FromBody] CreateSupplierDto dto, CancellationToken ct)
    {
        var res = await _service.CreateAsync(dto, ct);
        if (res.Success) return Ok(res);
        if (res.Message is "Supplier already exists with same name or code." or "Supplier already exists with same PAN.") return Conflict(res);
        return BadRequest(res);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<SupplierResponseDto>>> Update(long id, [FromBody] UpdateSupplierDto dto, CancellationToken ct)
    {
        var res = await _service.UpdateAsync(id, dto, ct);
        if (res.Success) return Ok(res);
        if (res.Message is "Supplier already exists with same name or code." or "Supplier already exists with same PAN.") return Conflict(res);
        return BadRequest(res);
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

