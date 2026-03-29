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
[Route("api/v{version:apiVersion}/pharmacy-sale")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrPharmacySale")]
public sealed class PharmacySaleController : ControllerBase
{
    private readonly IPhrPharmacySaleService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PharmacySaleController> _logger;

    public PharmacySaleController(IPhrPharmacySaleService service, ITenantContext tenant, ILogger<PharmacySaleController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "PharmacySale_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PharmacySaleResponseDto>))]
    public async Task<ActionResult<BaseResponse<PharmacySaleResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "PharmacySale_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PharmacySaleResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<PharmacySaleResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PharmacySaleResponseDto>>> Create([FromBody] CreatePharmacySaleDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PharmacySaleResponseDto>>> Update(long id, [FromBody] UpdatePharmacySaleDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
