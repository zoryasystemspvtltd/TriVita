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
[Route("api/v{version:apiVersion}/medicine-composition")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrMedicineComposition")]
public sealed class MedicineCompositionController : ControllerBase
{
    private readonly IPhrMedicineCompositionService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<MedicineCompositionController> _logger;

    public MedicineCompositionController(IPhrMedicineCompositionService service, ITenantContext tenant, ILogger<MedicineCompositionController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "MedicineComposition_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<MedicineCompositionResponseDto>))]
    public async Task<ActionResult<BaseResponse<MedicineCompositionResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "MedicineComposition_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<MedicineCompositionResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<MedicineCompositionResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<MedicineCompositionResponseDto>>> Create([FromBody] CreateMedicineCompositionDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<MedicineCompositionResponseDto>>> Update(long id, [FromBody] UpdateMedicineCompositionDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
