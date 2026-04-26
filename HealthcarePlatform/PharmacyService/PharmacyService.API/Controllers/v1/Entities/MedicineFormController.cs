using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/medicine-form")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrMedicineForm")]
public sealed class MedicineFormController : ControllerBase
{
    private readonly IPhrMedicineFormService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<MedicineFormController> _logger;

    public MedicineFormController(IPhrMedicineFormService service, ITenantContext tenant, ILogger<MedicineFormController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "MedicineForm_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<MedicineFormResponseDto>))]
    public async Task<ActionResult<BaseResponse<MedicineFormResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "MedicineForm_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<MedicineFormResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<MedicineFormResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<MedicineFormResponseDto>>> Create([FromBody] CreateMedicineFormDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<MedicineFormResponseDto>>> Update(long id, [FromBody] UpdateMedicineFormDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
