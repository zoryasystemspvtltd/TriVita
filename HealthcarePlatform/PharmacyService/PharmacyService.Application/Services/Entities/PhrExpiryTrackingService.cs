using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Domain.Entities;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Repositories;
using PharmacyService.Application.Services.Extended;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrExpiryTrackingService
{
    Task<BaseResponse<ExpiryTrackingResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ExpiryTrackingResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ExpiryTrackingResponseDto>> CreateAsync(CreateExpiryTrackingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ExpiryTrackingResponseDto>> UpdateAsync(long id, UpdateExpiryTrackingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrExpiryTrackingService : PhrCrudServiceBase<PhrExpiryTracking, CreateExpiryTrackingDto, UpdateExpiryTrackingDto, ExpiryTrackingResponseDto, PhrExpiryTrackingService>, IPhrExpiryTrackingService
{
    public PhrExpiryTrackingService(
        IRepository<PhrExpiryTracking> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateExpiryTrackingDto>? createValidator,
        IValidator<UpdateExpiryTrackingDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrExpiryTrackingService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<ExpiryTrackingResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
