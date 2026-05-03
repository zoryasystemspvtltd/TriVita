using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales-bill")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrSalesBill")]
public sealed class SalesBillController : ControllerBase
{
    private readonly IPhrSalesBillService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<SalesBillController> _logger;

    public SalesBillController(IPhrSalesBillService service, ITenantContext tenant, ILogger<SalesBillController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "SalesBill_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<SalesBillResponseDto>))]
    public async Task<ActionResult<BaseResponse<SalesBillResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("SalesBill GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "SalesBill_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<SalesBillResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<SalesBillResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<SalesBillResponseDto>>> Create([FromBody] CreateSalesBillDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<SalesBillResponseDto>>> Update(long id, [FromBody] UpdateSalesBillDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));

    [HttpPost("{id:long}/post")]
    [SwaggerOperation(OperationId = "SalesBill_Post")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<SalesBillResponseDto>))]
    public async Task<ActionResult<BaseResponse<SalesBillResponseDto>>> Post(long id, CancellationToken ct)
        => Ok(await _service.PostAsync(id, ct));
}
