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

public interface ILisSampleCollectionService
{
    Task<BaseResponse<SampleCollectionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SampleCollectionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleCollectionResponseDto>> CreateAsync(CreateSampleCollectionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleCollectionResponseDto>> UpdateAsync(long id, UpdateSampleCollectionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisSampleCollectionService : LisCrudServiceBase<LisSampleCollection, CreateSampleCollectionDto, UpdateSampleCollectionDto, SampleCollectionResponseDto, LisSampleCollectionService>, ILisSampleCollectionService
{
    public LisSampleCollectionService(
        IRepository<LisSampleCollection> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateSampleCollectionDto>? createValidator,
        IValidator<UpdateSampleCollectionDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisSampleCollectionService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<SampleCollectionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
