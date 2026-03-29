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

public interface ILisSampleTypeService
{
    Task<BaseResponse<SampleTypeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SampleTypeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleTypeResponseDto>> CreateAsync(CreateSampleTypeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SampleTypeResponseDto>> UpdateAsync(long id, UpdateSampleTypeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisSampleTypeService : LisCrudServiceBase<LisSampleType, CreateSampleTypeDto, UpdateSampleTypeDto, SampleTypeResponseDto, LisSampleTypeService>, ILisSampleTypeService
{
    public LisSampleTypeService(
        IRepository<LisSampleType> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateSampleTypeDto>? createValidator,
        IValidator<UpdateSampleTypeDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisSampleTypeService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<SampleTypeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
