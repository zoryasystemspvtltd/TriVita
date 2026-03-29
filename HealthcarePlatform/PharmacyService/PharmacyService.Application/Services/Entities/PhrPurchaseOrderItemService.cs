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

public interface IPhrPurchaseOrderItemService
{
    Task<BaseResponse<PurchaseOrderItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PurchaseOrderItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PurchaseOrderItemResponseDto>> CreateAsync(CreatePurchaseOrderItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PurchaseOrderItemResponseDto>> UpdateAsync(long id, UpdatePurchaseOrderItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrPurchaseOrderItemService : PhrCrudServiceBase<PhrPurchaseOrderItem, CreatePurchaseOrderItemDto, UpdatePurchaseOrderItemDto, PurchaseOrderItemResponseDto, PhrPurchaseOrderItemService>, IPhrPurchaseOrderItemService
{
    public PhrPurchaseOrderItemService(
        IRepository<PhrPurchaseOrderItem> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePurchaseOrderItemDto>? createValidator,
        IValidator<UpdatePurchaseOrderItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrPurchaseOrderItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<PurchaseOrderItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
