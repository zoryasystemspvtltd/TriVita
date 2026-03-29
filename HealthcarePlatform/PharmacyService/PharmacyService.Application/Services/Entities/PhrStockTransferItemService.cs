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

public interface IPhrStockTransferItemService
{
    Task<BaseResponse<StockTransferItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockTransferItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockTransferItemResponseDto>> CreateAsync(CreateStockTransferItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockTransferItemResponseDto>> UpdateAsync(long id, UpdateStockTransferItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockTransferItemService : PhrCrudServiceBase<PhrStockTransferItem, CreateStockTransferItemDto, UpdateStockTransferItemDto, StockTransferItemResponseDto, PhrStockTransferItemService>, IPhrStockTransferItemService
{
    public PhrStockTransferItemService(
        IRepository<PhrStockTransferItem> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateStockTransferItemDto>? createValidator,
        IValidator<UpdateStockTransferItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrStockTransferItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockTransferItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
