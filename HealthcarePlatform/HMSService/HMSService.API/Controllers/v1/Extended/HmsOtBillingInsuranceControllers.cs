using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using HMSService.Application.DTOs.Gap;
using HMSService.Application.Services.Gap;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1.Extended;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/ot/operation-theatres")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS operation theatres (script 10)")]
public sealed class OperationTheatresController : ControllerBase
{
    private readonly IOperationTheatreService _service;

    public OperationTheatresController(IOperationTheatreService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<OperationTheatreResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<OperationTheatreResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<OperationTheatreResponseDto>>> Create(
        [FromBody] CreateOperationTheatreDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<OperationTheatreResponseDto>>> Update(
        long id,
        [FromBody] UpdateOperationTheatreDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/ot/surgery-schedules")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS surgery schedules")]
public sealed class SurgerySchedulesController : ControllerBase
{
    private readonly ISurgeryScheduleService _service;

    public SurgerySchedulesController(ISurgeryScheduleService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<SurgeryScheduleResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<SurgeryScheduleResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? operationTheatreId,
        [FromQuery] long? patientMasterId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, operationTheatreId, patientMasterId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<SurgeryScheduleResponseDto>>> Create(
        [FromBody] CreateSurgeryScheduleDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<SurgeryScheduleResponseDto>>> Update(
        long id,
        [FromBody] UpdateSurgeryScheduleDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/ot/anesthesia-records")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class AnesthesiaRecordsController : ControllerBase
{
    private readonly IAnesthesiaRecordService _service;

    public AnesthesiaRecordsController(IAnesthesiaRecordService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<AnesthesiaRecordResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<AnesthesiaRecordResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? surgeryScheduleId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, surgeryScheduleId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<AnesthesiaRecordResponseDto>>> Create(
        [FromBody] CreateAnesthesiaRecordDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<AnesthesiaRecordResponseDto>>> Update(
        long id,
        [FromBody] UpdateAnesthesiaRecordDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/ot/post-op-records")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class PostOpRecordsController : ControllerBase
{
    private readonly IPostOpRecordService _service;

    public PostOpRecordsController(IPostOpRecordService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PostOpRecordResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PostOpRecordResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? surgeryScheduleId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, surgeryScheduleId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PostOpRecordResponseDto>>> Create(
        [FromBody] CreatePostOpRecordDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PostOpRecordResponseDto>>> Update(
        long id,
        [FromBody] UpdatePostOpRecordDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/ot/consumables")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class OtConsumablesController : ControllerBase
{
    private readonly IOtConsumableService _service;

    public OtConsumablesController(IOtConsumableService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<OtConsumableResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<OtConsumableResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? surgeryScheduleId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, surgeryScheduleId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<OtConsumableResponseDto>>> Create(
        [FromBody] CreateOtConsumableDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<OtConsumableResponseDto>>> Update(
        long id,
        [FromBody] UpdateOtConsumableDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/billing/pricing-rules")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class PricingRulesController : ControllerBase
{
    private readonly IPricingRuleService _service;

    public PricingRulesController(IPricingRuleService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PricingRuleResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PricingRuleResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PricingRuleResponseDto>>> Create(
        [FromBody] CreatePricingRuleDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PricingRuleResponseDto>>> Update(
        long id,
        [FromBody] UpdatePricingRuleDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/billing/package-definitions")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class PackageDefinitionsController : ControllerBase
{
    private readonly IPackageDefinitionService _service;

    public PackageDefinitionsController(IPackageDefinitionService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PackageDefinitionResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PackageDefinitionResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PackageDefinitionResponseDto>>> Create(
        [FromBody] CreatePackageDefinitionDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PackageDefinitionResponseDto>>> Update(
        long id,
        [FromBody] UpdatePackageDefinitionDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/billing/package-definition-lines")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class PackageDefinitionLinesController : ControllerBase
{
    private readonly IPackageDefinitionLineService _service;

    public PackageDefinitionLinesController(IPackageDefinitionLineService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PackageDefinitionLineResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PackageDefinitionLineResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? packageDefinitionId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, packageDefinitionId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PackageDefinitionLineResponseDto>>> Create(
        [FromBody] CreatePackageDefinitionLineDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PackageDefinitionLineResponseDto>>> Update(
        long id,
        [FromBody] UpdatePackageDefinitionLineDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/billing/proforma-invoices")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class ProformaInvoicesController : ControllerBase
{
    private readonly IProformaInvoiceService _service;

    public ProformaInvoicesController(IProformaInvoiceService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<ProformaInvoiceResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<ProformaInvoiceResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ProformaInvoiceResponseDto>>> Create(
        [FromBody] CreateProformaInvoiceDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ProformaInvoiceResponseDto>>> Update(
        long id,
        [FromBody] UpdateProformaInvoiceDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/insurance/providers")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class InsuranceProvidersController : ControllerBase
{
    private readonly IInsuranceProviderService _service;

    public InsuranceProvidersController(IInsuranceProviderService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<InsuranceProviderResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<InsuranceProviderResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<InsuranceProviderResponseDto>>> Create(
        [FromBody] CreateInsuranceProviderDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<InsuranceProviderResponseDto>>> Update(
        long id,
        [FromBody] UpdateInsuranceProviderDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/insurance/pre-authorizations")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class PreAuthorizationsController : ControllerBase
{
    private readonly IPreAuthorizationService _service;

    public PreAuthorizationsController(IPreAuthorizationService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PreAuthorizationResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PreAuthorizationResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? insuranceProviderId,
        [FromQuery] long? patientMasterId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, insuranceProviderId, patientMasterId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PreAuthorizationResponseDto>>> Create(
        [FromBody] CreatePreAuthorizationDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PreAuthorizationResponseDto>>> Update(
        long id,
        [FromBody] UpdatePreAuthorizationDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/insurance/claims")]
[RequirePermission(TriVitaPermissions.HmsApi)]
public sealed class InsuranceClaimsController : ControllerBase
{
    private readonly IClaimService _service;

    public InsuranceClaimsController(IClaimService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<ClaimResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<ClaimResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? insuranceProviderId,
        [FromQuery] long? patientMasterId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, insuranceProviderId, patientMasterId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ClaimResponseDto>>> Create(
        [FromBody] CreateClaimDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ClaimResponseDto>>> Update(
        long id,
        [FromBody] UpdateClaimDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}
