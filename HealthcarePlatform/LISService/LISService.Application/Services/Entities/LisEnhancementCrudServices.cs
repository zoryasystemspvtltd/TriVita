using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.Application.DTOs.Entities;
using LISService.Application.Services.Extended;
using LISService.Domain.Entities;
using LISService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LISService.Application.Services.Entities;

public interface ILisTestParameterProfileService
{
    Task<BaseResponse<TestParameterProfileResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestParameterProfileResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestParameterProfileResponseDto>> CreateAsync(CreateTestParameterProfileDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestParameterProfileResponseDto>> UpdateAsync(long id, UpdateTestParameterProfileDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisTestParameterProfileService
    : LisCrudServiceBase<LisTestParameterProfile, CreateTestParameterProfileDto, UpdateTestParameterProfileDto, TestParameterProfileResponseDto, LisTestParameterProfileService>,
        ILisTestParameterProfileService
{
    public LisTestParameterProfileService(
        IRepository<LisTestParameterProfile> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateTestParameterProfileDto>? createValidator,
        IValidator<UpdateTestParameterProfileDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisTestParameterProfileService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<TestParameterProfileResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILisAnalyzerResultMapService
{
    Task<BaseResponse<AnalyzerResultMapResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<AnalyzerResultMapResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<AnalyzerResultMapResponseDto>> CreateAsync(CreateAnalyzerResultMapDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<AnalyzerResultMapResponseDto>> UpdateAsync(long id, UpdateAnalyzerResultMapDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisAnalyzerResultMapService
    : LisCrudServiceBase<LisAnalyzerResultMap, CreateAnalyzerResultMapDto, UpdateAnalyzerResultMapDto, AnalyzerResultMapResponseDto, LisAnalyzerResultMapService>,
        ILisAnalyzerResultMapService
{
    public LisAnalyzerResultMapService(
        IRepository<LisAnalyzerResultMap> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateAnalyzerResultMapDto>? createValidator,
        IValidator<UpdateAnalyzerResultMapDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisAnalyzerResultMapService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<AnalyzerResultMapResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILisSampleBarcodeService
{
    Task<BaseResponse<SampleBarcodeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SampleBarcodeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleBarcodeResponseDto>> CreateAsync(CreateSampleBarcodeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleBarcodeResponseDto>> UpdateAsync(long id, UpdateSampleBarcodeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisSampleBarcodeService
    : LisCrudServiceBase<LisSampleBarcode, CreateSampleBarcodeDto, UpdateSampleBarcodeDto, SampleBarcodeResponseDto, LisSampleBarcodeService>,
        ILisSampleBarcodeService
{
    public LisSampleBarcodeService(
        IRepository<LisSampleBarcode> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateSampleBarcodeDto>? createValidator,
        IValidator<UpdateSampleBarcodeDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisSampleBarcodeService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<SampleBarcodeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILisSampleLifecycleEventService
{
    Task<BaseResponse<SampleLifecycleEventResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SampleLifecycleEventResponseDto>>> GetPagedAsync(PagedQuery query, long? sampleCollectionId, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleLifecycleEventResponseDto>> CreateAsync(CreateSampleLifecycleEventDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleLifecycleEventResponseDto>> UpdateAsync(long id, UpdateSampleLifecycleEventDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisSampleLifecycleEventService
    : LisCrudServiceBase<LisSampleLifecycleEvent, CreateSampleLifecycleEventDto, UpdateSampleLifecycleEventDto, SampleLifecycleEventResponseDto, LisSampleLifecycleEventService>,
        ILisSampleLifecycleEventService
{
    public LisSampleLifecycleEventService(
        IRepository<LisSampleLifecycleEvent> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateSampleLifecycleEventDto>? createValidator,
        IValidator<UpdateSampleLifecycleEventDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisSampleLifecycleEventService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<SampleLifecycleEventResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? sampleCollectionId,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<LisSampleLifecycleEvent, bool>>? filter =
            sampleCollectionId is { } sid ? e => e.SampleCollectionId == sid : null;
        return GetPagedCoreAsync(query, filter, cancellationToken);
    }
}

public interface ILisReportDeliveryOtpService
{
    Task<BaseResponse<ReportDeliveryOtpResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReportDeliveryOtpResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportDeliveryOtpResponseDto>> CreateAsync(CreateReportDeliveryOtpDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportDeliveryOtpResponseDto>> UpdateAsync(long id, UpdateReportDeliveryOtpDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisReportDeliveryOtpService
    : LisCrudServiceBase<LisReportDeliveryOtp, CreateReportDeliveryOtpDto, UpdateReportDeliveryOtpDto, ReportDeliveryOtpResponseDto, LisReportDeliveryOtpService>,
        ILisReportDeliveryOtpService
{
    public LisReportDeliveryOtpService(
        IRepository<LisReportDeliveryOtp> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateReportDeliveryOtpDto>? createValidator,
        IValidator<UpdateReportDeliveryOtpDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisReportDeliveryOtpService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<ReportDeliveryOtpResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILisReportLockStateService
{
    Task<BaseResponse<ReportLockStateResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReportLockStateResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportLockStateResponseDto>> CreateAsync(CreateReportLockStateDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportLockStateResponseDto>> UpdateAsync(long id, UpdateReportLockStateDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisReportLockStateService
    : LisCrudServiceBase<LisReportLockState, CreateReportLockStateDto, UpdateReportLockStateDto, ReportLockStateResponseDto, LisReportLockStateService>,
        ILisReportLockStateService
{
    public LisReportLockStateService(
        IRepository<LisReportLockState> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateReportLockStateDto>? createValidator,
        IValidator<UpdateReportLockStateDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisReportLockStateService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<ReportLockStateResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
