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
[Route("api/v{version:apiVersion}/medicine-category")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrMedicineCategory")]
public sealed class MedicineCategoryController : ControllerBase
{
    private readonly IPhrMedicineCategoryService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<MedicineCategoryController> _logger;

    public MedicineCategoryController(IPhrMedicineCategoryService service, ITenantContext tenant, ILogger<MedicineCategoryController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "MedicineCategory_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<MedicineCategoryResponseDto>))]
    public async Task<ActionResult<BaseResponse<MedicineCategoryResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "MedicineCategory_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<MedicineCategoryResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<MedicineCategoryResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<MedicineCategoryResponseDto>>> Create([FromBody] CreateMedicineCategoryDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<MedicineCategoryResponseDto>>> Update(long id, [FromBody] UpdateMedicineCategoryDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
