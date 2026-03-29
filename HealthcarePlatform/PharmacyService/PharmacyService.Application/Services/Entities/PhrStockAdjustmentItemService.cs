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

public interface IPhrStockAdjustmentItemService
{
    Task<BaseResponse<StockAdjustmentItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockAdjustmentItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentItemResponseDto>> CreateAsync(CreateStockAdjustmentItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentItemResponseDto>> UpdateAsync(long id, UpdateStockAdjustmentItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockAdjustmentItemService : PhrCrudServiceBase<PhrStockAdjustmentItem, CreateStockAdjustmentItemDto, UpdateStockAdjustmentItemDto, StockAdjustmentItemResponseDto, PhrStockAdjustmentItemService>, IPhrStockAdjustmentItemService
{
    public PhrStockAdjustmentItemService(
        IRepository<PhrStockAdjustmentItem> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateStockAdjustmentItemDto>? createValidator,
        IValidator<UpdateStockAdjustmentItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrStockAdjustmentItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockAdjustmentItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
