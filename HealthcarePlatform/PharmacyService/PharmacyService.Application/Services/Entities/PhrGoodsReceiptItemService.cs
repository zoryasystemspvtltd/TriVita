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

public interface IPhrGoodsReceiptItemService
{
    Task<BaseResponse<GoodsReceiptItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<GoodsReceiptItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<GoodsReceiptItemResponseDto>> CreateAsync(CreateGoodsReceiptItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<GoodsReceiptItemResponseDto>> UpdateAsync(long id, UpdateGoodsReceiptItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrGoodsReceiptItemService : PhrCrudServiceBase<PhrGoodsReceiptItem, CreateGoodsReceiptItemDto, UpdateGoodsReceiptItemDto, GoodsReceiptItemResponseDto, PhrGoodsReceiptItemService>, IPhrGoodsReceiptItemService
{
    public PhrGoodsReceiptItemService(
        IRepository<PhrGoodsReceiptItem> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateGoodsReceiptItemDto>? createValidator,
        IValidator<UpdateGoodsReceiptItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrGoodsReceiptItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<GoodsReceiptItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
