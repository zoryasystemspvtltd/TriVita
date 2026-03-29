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

public interface IPhrPharmacySalesItemService
{
    Task<BaseResponse<PharmacySalesItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PharmacySalesItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PharmacySalesItemResponseDto>> CreateAsync(CreatePharmacySalesItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PharmacySalesItemResponseDto>> UpdateAsync(long id, UpdatePharmacySalesItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrPharmacySalesItemService : PhrCrudServiceBase<PhrPharmacySalesItem, CreatePharmacySalesItemDto, UpdatePharmacySalesItemDto, PharmacySalesItemResponseDto, PhrPharmacySalesItemService>, IPhrPharmacySalesItemService
{
    public PhrPharmacySalesItemService(
        IRepository<PhrPharmacySalesItem> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePharmacySalesItemDto>? createValidator,
        IValidator<UpdatePharmacySalesItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrPharmacySalesItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<PharmacySalesItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
