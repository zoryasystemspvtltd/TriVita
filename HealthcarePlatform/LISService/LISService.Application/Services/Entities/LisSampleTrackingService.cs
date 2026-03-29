using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.Domain.Entities;
using LISService.Application.DTOs.Entities;
using LISService.Domain.Repositories;
using LISService.Application.Services.Extended;
using Microsoft.Extensions.Logging;

namespace LISService.Application.Services.Entities;

public interface ILisSampleTrackingService
{
    Task<BaseResponse<SampleTrackingResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SampleTrackingResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleTrackingResponseDto>> CreateAsync(CreateSampleTrackingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleTrackingResponseDto>> UpdateAsync(long id, UpdateSampleTrackingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisSampleTrackingService : LisCrudServiceBase<LisSampleTracking, CreateSampleTrackingDto, UpdateSampleTrackingDto, SampleTrackingResponseDto, LisSampleTrackingService>, ILisSampleTrackingService
{
    public LisSampleTrackingService(
        IRepository<LisSampleTracking> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateSampleTrackingDto>? createValidator,
        IValidator<UpdateSampleTrackingDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisSampleTrackingService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<SampleTrackingResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
