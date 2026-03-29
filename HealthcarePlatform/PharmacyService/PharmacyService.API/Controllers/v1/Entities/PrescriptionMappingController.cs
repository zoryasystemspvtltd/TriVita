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
[Route("api/v{version:apiVersion}/prescription-mapping")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrPrescriptionMapping")]
public sealed class PrescriptionMappingController : ControllerBase
{
    private readonly IPhrPrescriptionMappingService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PrescriptionMappingController> _logger;

    public PrescriptionMappingController(IPhrPrescriptionMappingService service, ITenantContext tenant, ILogger<PrescriptionMappingController> logger)
    { _service = service; _tenant = tenant; _logger = logger; }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "PrescriptionMapping_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PrescriptionMappingResponseDto>))]
    public async Task<ActionResult<BaseResponse<PrescriptionMappingResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "PrescriptionMapping_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<PrescriptionMappingResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<PrescriptionMappingResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PrescriptionMappingResponseDto>>> Create([FromBody] CreatePrescriptionMappingDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PrescriptionMappingResponseDto>>> Update(long id, [FromBody] UpdatePrescriptionMappingDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
