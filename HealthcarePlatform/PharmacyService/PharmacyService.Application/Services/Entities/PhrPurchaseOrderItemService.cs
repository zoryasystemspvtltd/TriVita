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
    private readonly IRepository<PhrPurchaseOrder> _purchaseOrders;
    private readonly IRepository<PhrPurchaseOrderItem> _items;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrPurchaseOrderItemService(
        IRepository<PhrPurchaseOrderItem> repository,
        IRepository<PhrPurchaseOrder> purchaseOrders,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePurchaseOrderItemDto>? createValidator,
        IValidator<UpdatePurchaseOrderItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrPurchaseOrderItemService> logger,
        IPharmacyUnitOfWork uow)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _purchaseOrders = purchaseOrders;
        _items = repository;
        _uow = uow;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<PurchaseOrderItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<PurchaseOrderItemResponseDto>> CreateAsync(
        CreatePurchaseOrderItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantityOrdered <= 0) return BaseResponse<PurchaseOrderItemResponseDto>.Fail("Quantity must be greater than zero.");
        if (dto.UnitPrice <= 0) return BaseResponse<PurchaseOrderItemResponseDto>.Fail("UnitPrice must be greater than zero.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var entity = Mapper.Map<PhrPurchaseOrderItem>(dto);
                entity.PurchaseRate = dto.UnitPrice;
                entity.LineTotal = dto.QuantityOrdered * dto.UnitPrice;

                AuditHelper.ApplyCreate(entity, Tenant);
                entity.FacilityId = Tenant.FacilityId;

                await _items.AddAsync(entity, ct);
                await _items.SaveChangesAsync(ct);

                await RecalcPurchaseOrderTotalsAsync(entity.PurchaseOrderId, ct);

                return BaseResponse<PurchaseOrderItemResponseDto>.Ok(Mapper.Map<PurchaseOrderItemResponseDto>(entity), "Created.");
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PO item create failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<PurchaseOrderItemResponseDto>.Fail("Create failed.");
        }
    }

    public override async Task<BaseResponse<PurchaseOrderItemResponseDto>> UpdateAsync(
        long id,
        UpdatePurchaseOrderItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantityOrdered <= 0) return BaseResponse<PurchaseOrderItemResponseDto>.Fail("Quantity must be greater than zero.");
        if (dto.UnitPrice <= 0) return BaseResponse<PurchaseOrderItemResponseDto>.Fail("UnitPrice must be greater than zero.");

        var entity = await _items.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<PurchaseOrderItemResponseDto>.Fail("PurchaseOrderItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<PurchaseOrderItemResponseDto>.Fail("Resource is not in the current facility scope.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                Mapper.Map(dto, entity);
                entity.PurchaseRate = dto.UnitPrice;
                entity.LineTotal = dto.QuantityOrdered * dto.UnitPrice;
                AuditHelper.ApplyUpdate(entity, Tenant);

                await _items.UpdateAsync(entity, ct);
                await _items.SaveChangesAsync(ct);

                await RecalcPurchaseOrderTotalsAsync(entity.PurchaseOrderId, ct);

                return BaseResponse<PurchaseOrderItemResponseDto>.Ok(Mapper.Map<PurchaseOrderItemResponseDto>(entity), "Updated.");
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PO item update failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<PurchaseOrderItemResponseDto>.Fail("Update failed.");
        }
    }

    public override async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _items.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<object?>.Fail("PurchaseOrderItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                entity.IsDeleted = true;
                entity.IsActive = false;
                AuditHelper.ApplyUpdate(entity, Tenant);

                await _items.UpdateAsync(entity, ct);
                await _items.SaveChangesAsync(ct);

                await RecalcPurchaseOrderTotalsAsync(entity.PurchaseOrderId, ct);

                return BaseResponse<object?>.Ok(null, "Deleted.");
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PO item delete failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<object?>.Fail("Delete failed.");
        }
    }

    private async Task RecalcPurchaseOrderTotalsAsync(long purchaseOrderId, CancellationToken ct)
    {
        var po = await _purchaseOrders.GetByIdAsync(purchaseOrderId, ct);
        if (po is null) return;

        var lines = await _items.ListAsync(x => x.PurchaseOrderId == purchaseOrderId, ct);
        var subTotal = lines.Sum(x => x.LineTotal);

        po.SubTotal = subTotal;
        var taxable = Math.Max(0m, po.SubTotal - po.DiscountAmount);
        po.GstAmount = Math.Round(taxable * (po.GstPercent / 100m), 4, MidpointRounding.AwayFromZero);
        po.TotalAmount = Math.Round(po.SubTotal - po.DiscountAmount + po.GstAmount + po.OtherTaxAmount, 4, MidpointRounding.AwayFromZero);

        AuditHelper.ApplyUpdate(po, Tenant);
        await _purchaseOrders.UpdateAsync(po, ct);
        await _purchaseOrders.SaveChangesAsync(ct);
    }
}
