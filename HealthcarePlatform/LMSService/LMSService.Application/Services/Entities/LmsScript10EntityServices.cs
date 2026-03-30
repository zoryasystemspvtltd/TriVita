using AutoMapper;
using FluentValidation;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Extended;
using LMSService.Domain.Entities;
using LMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMSService.Application.Services.Entities;

public interface ILmsCollectionRequestService
{
    Task<BaseResponse<LmsCollectionRequestResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsCollectionRequestResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsCollectionRequestResponseDto>> CreateAsync(CreateLmsCollectionRequestDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsCollectionRequestResponseDto>> UpdateAsync(long id, UpdateLmsCollectionRequestDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsCollectionRequestService
    : LmsCrudServiceBase<LmsCollectionRequest, CreateLmsCollectionRequestDto, UpdateLmsCollectionRequestDto, LmsCollectionRequestResponseDto, LmsCollectionRequestService>,
        ILmsCollectionRequestService
{
    public LmsCollectionRequestService(
        IRepository<LmsCollectionRequest> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsCollectionRequestDto>? cv,
        IValidator<UpdateLmsCollectionRequestDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<LmsCollectionRequestService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    protected override Task OnBeforeCreateAsync(
        LmsCollectionRequest entity,
        CreateLmsCollectionRequestDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity.RequestNo))
        {
            var f = Tenant.FacilityId ?? 0;
            entity.RequestNo = $"COL-{f}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        }

        return Task.CompletedTask;
    }

    public Task<BaseResponse<PagedResponse<LmsCollectionRequestResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(query, null, ct);
}

public interface ILmsRiderTrackingService
{
    Task<BaseResponse<LmsRiderTrackingResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsRiderTrackingResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? collectionRequestId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsRiderTrackingResponseDto>> CreateAsync(CreateLmsRiderTrackingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsRiderTrackingResponseDto>> UpdateAsync(long id, UpdateLmsRiderTrackingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsRiderTrackingService
    : LmsCrudServiceBase<LmsRiderTracking, CreateLmsRiderTrackingDto, UpdateLmsRiderTrackingDto, LmsRiderTrackingResponseDto, LmsRiderTrackingService>,
        ILmsRiderTrackingService
{
    public LmsRiderTrackingService(
        IRepository<LmsRiderTracking> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsRiderTrackingDto>? cv,
        IValidator<UpdateLmsRiderTrackingDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<LmsRiderTrackingService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LmsRiderTrackingResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? collectionRequestId,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(
            query,
            collectionRequestId is null ? null : e => e.CollectionRequestId == collectionRequestId.Value,
            ct);
}

public interface ILmsSampleTransportService
{
    Task<BaseResponse<LmsSampleTransportResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsSampleTransportResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? collectionRequestId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsSampleTransportResponseDto>> CreateAsync(CreateLmsSampleTransportDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsSampleTransportResponseDto>> UpdateAsync(long id, UpdateLmsSampleTransportDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsSampleTransportService
    : LmsCrudServiceBase<LmsSampleTransport, CreateLmsSampleTransportDto, UpdateLmsSampleTransportDto, LmsSampleTransportResponseDto, LmsSampleTransportService>,
        ILmsSampleTransportService
{
    public LmsSampleTransportService(
        IRepository<LmsSampleTransport> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsSampleTransportDto>? cv,
        IValidator<UpdateLmsSampleTransportDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<LmsSampleTransportService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LmsSampleTransportResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? collectionRequestId,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(
            query,
            collectionRequestId is null ? null : e => e.CollectionRequestId == collectionRequestId.Value,
            ct);
}

public interface ILmsReportValidationStepService
{
    Task<BaseResponse<LmsReportValidationStepResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsReportValidationStepResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? labOrderId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsReportValidationStepResponseDto>> CreateAsync(CreateLmsReportValidationStepDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsReportValidationStepResponseDto>> UpdateAsync(long id, UpdateLmsReportValidationStepDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReportValidationStepService
    : LmsCrudServiceBase<LmsReportValidationStep, CreateLmsReportValidationStepDto, UpdateLmsReportValidationStepDto, LmsReportValidationStepResponseDto, LmsReportValidationStepService>,
        ILmsReportValidationStepService
{
    public LmsReportValidationStepService(
        IRepository<LmsReportValidationStep> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsReportValidationStepDto>? cv,
        IValidator<UpdateLmsReportValidationStepDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<LmsReportValidationStepService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LmsReportValidationStepResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? labOrderId,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(
            query,
            labOrderId is null ? null : e => e.LabOrderId == labOrderId.Value,
            ct);
}

public interface ILmsResultDeltaCheckService
{
    Task<BaseResponse<LmsResultDeltaCheckResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsResultDeltaCheckResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsResultDeltaCheckResponseDto>> CreateAsync(CreateLmsResultDeltaCheckDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsResultDeltaCheckResponseDto>> UpdateAsync(long id, UpdateLmsResultDeltaCheckDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsResultDeltaCheckService
    : LmsCrudServiceBase<LmsResultDeltaCheck, CreateLmsResultDeltaCheckDto, UpdateLmsResultDeltaCheckDto, LmsResultDeltaCheckResponseDto, LmsResultDeltaCheckService>,
        ILmsResultDeltaCheckService
{
    public LmsResultDeltaCheckService(
        IRepository<LmsResultDeltaCheck> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsResultDeltaCheckDto>? cv,
        IValidator<UpdateLmsResultDeltaCheckDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<LmsResultDeltaCheckService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LmsResultDeltaCheckResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(query, null, ct);
}

public interface ILmsReportDigitalSignService
{
    Task<BaseResponse<LmsReportDigitalSignResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsReportDigitalSignResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? reportHeaderId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsReportDigitalSignResponseDto>> CreateAsync(CreateLmsReportDigitalSignDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsReportDigitalSignResponseDto>> UpdateAsync(long id, UpdateLmsReportDigitalSignDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReportDigitalSignService
    : LmsCrudServiceBase<LmsReportDigitalSign, CreateLmsReportDigitalSignDto, UpdateLmsReportDigitalSignDto, LmsReportDigitalSignResponseDto, LmsReportDigitalSignService>,
        ILmsReportDigitalSignService
{
    public LmsReportDigitalSignService(
        IRepository<LmsReportDigitalSign> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsReportDigitalSignDto>? cv,
        IValidator<UpdateLmsReportDigitalSignDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<LmsReportDigitalSignService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LmsReportDigitalSignResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? reportHeaderId,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(
            query,
            reportHeaderId is null ? null : e => e.ReportHeaderId == reportHeaderId.Value,
            ct);
}
