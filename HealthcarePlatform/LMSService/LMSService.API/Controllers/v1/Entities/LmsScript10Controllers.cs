using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lms/collection-requests")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS collection requests (script 10)")]
public sealed class LmsCollectionRequestsController : ControllerBase
{
    private readonly ILmsCollectionRequestService _service;

    public LmsCollectionRequestsController(ILmsCollectionRequestService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsCollectionRequestResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsCollectionRequestResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsCollectionRequestResponseDto>>> Create(
        [FromBody] CreateLmsCollectionRequestDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsCollectionRequestResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsCollectionRequestDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lms/rider-tracking")]
[RequirePermission(TriVitaPermissions.LmsApi)]
public sealed class LmsRiderTrackingController : ControllerBase
{
    private readonly ILmsRiderTrackingService _service;

    public LmsRiderTrackingController(ILmsRiderTrackingService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsRiderTrackingResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsRiderTrackingResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? collectionRequestId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, collectionRequestId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsRiderTrackingResponseDto>>> Create(
        [FromBody] CreateLmsRiderTrackingDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsRiderTrackingResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsRiderTrackingDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lms/sample-transport")]
[RequirePermission(TriVitaPermissions.LmsApi)]
public sealed class LmsSampleTransportController : ControllerBase
{
    private readonly ILmsSampleTransportService _service;

    public LmsSampleTransportController(ILmsSampleTransportService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsSampleTransportResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsSampleTransportResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? collectionRequestId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, collectionRequestId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsSampleTransportResponseDto>>> Create(
        [FromBody] CreateLmsSampleTransportDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsSampleTransportResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsSampleTransportDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lms/report-validation-steps")]
[RequirePermission(TriVitaPermissions.LmsApi)]
public sealed class LmsReportValidationStepsController : ControllerBase
{
    private readonly ILmsReportValidationStepService _service;

    public LmsReportValidationStepsController(ILmsReportValidationStepService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsReportValidationStepResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsReportValidationStepResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? labOrderId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, labOrderId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsReportValidationStepResponseDto>>> Create(
        [FromBody] CreateLmsReportValidationStepDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsReportValidationStepResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsReportValidationStepDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lms/result-delta-checks")]
[RequirePermission(TriVitaPermissions.LmsApi)]
public sealed class LmsResultDeltaChecksController : ControllerBase
{
    private readonly ILmsResultDeltaCheckService _service;

    public LmsResultDeltaChecksController(ILmsResultDeltaCheckService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsResultDeltaCheckResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsResultDeltaCheckResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsResultDeltaCheckResponseDto>>> Create(
        [FromBody] CreateLmsResultDeltaCheckDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsResultDeltaCheckResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsResultDeltaCheckDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lms/report-digital-signs")]
[RequirePermission(TriVitaPermissions.LmsApi)]
public sealed class LmsReportDigitalSignsController : ControllerBase
{
    private readonly ILmsReportDigitalSignService _service;

    public LmsReportDigitalSignsController(ILmsReportDigitalSignService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsReportDigitalSignResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LmsReportDigitalSignResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? reportHeaderId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, reportHeaderId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsReportDigitalSignResponseDto>>> Create(
        [FromBody] CreateLmsReportDigitalSignDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LmsReportDigitalSignResponseDto>>> Update(
        long id,
        [FromBody] UpdateLmsReportDigitalSignDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}
