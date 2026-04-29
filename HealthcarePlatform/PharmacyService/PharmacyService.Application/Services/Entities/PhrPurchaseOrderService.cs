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

public interface IPhrPurchaseOrderService
{
    Task<BaseResponse<PurchaseOrderResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PurchaseOrderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PurchaseOrderResponseDto>> CreateAsync(CreatePurchaseOrderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PurchaseOrderResponseDto>> UpdateAsync(long id, UpdatePurchaseOrderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrPurchaseOrderService : PhrCrudServiceBase<PhrPurchaseOrder, CreatePurchaseOrderDto, UpdatePurchaseOrderDto, PurchaseOrderResponseDto, PhrPurchaseOrderService>, IPhrPurchaseOrderService
{
    private readonly IRepository<PhrPurchaseOrder> _purchaseOrders;
    private readonly IRepository<PhrPurchaseOrderItem> _items;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrPurchaseOrderService(
        IRepository<PhrPurchaseOrder> repository,
        IRepository<PhrPurchaseOrderItem> items,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePurchaseOrderDto>? createValidator,
        IValidator<UpdatePurchaseOrderDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrPurchaseOrderService> logger,
        IPharmacyUnitOfWork uow)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _purchaseOrders = repository;
        _items = items;
        _uow = uow;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<PurchaseOrderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<PurchaseOrderResponseDto>> CreateAsync(
        CreatePurchaseOrderDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var poNo = (dto.PurchaseOrderNo ?? string.Empty).Trim();
                var year = (dto.OrderDate == default ? DateTime.UtcNow : dto.OrderDate).Year;

                if (string.IsNullOrWhiteSpace(poNo))
                    poNo = await GenerateNextPoNumberAsync(year, ct);

                var dup = await _purchaseOrders.ListAsync(x => x.PurchaseOrderNo == poNo, ct);
                if (dup.Count > 0)
                    return BaseResponse<PurchaseOrderResponseDto>.Fail("Duplicate PO number.");

                dto.PurchaseOrderNo = poNo;

                var res = await base.CreateAsync(dto, ct);
                if (!res.Success || res.Data is null) return res;

                await RecalcTotalsAsync(res.Data.Id, ct);
                var po = await _purchaseOrders.GetByIdAsync(res.Data.Id, ct);
                return po is null
                    ? BaseResponse<PurchaseOrderResponseDto>.Fail("PurchaseOrder not found.")
                    : BaseResponse<PurchaseOrderResponseDto>.Ok(Mapper.Map<PurchaseOrderResponseDto>(po), "Created.");
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PO create failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<PurchaseOrderResponseDto>.Fail("Create failed.");
        }
    }

    private async Task<string> GenerateNextPoNumberAsync(int year, CancellationToken ct)
    {
        var prefix = $"PO/{year}/";
        var all = await _purchaseOrders.ListAsync(x => x.PurchaseOrderNo.StartsWith(prefix), ct);
        var max = 0;
        foreach (var po in all)
        {
            var s = po.PurchaseOrderNo ?? string.Empty;
            if (!s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
            var tail = s.Substring(prefix.Length);
            if (tail.Length != 4) continue;
            if (int.TryParse(tail, out var n) && n > max) max = n;
        }
        return $"{prefix}{(max + 1):D4}";
    }

    public override async Task<BaseResponse<PurchaseOrderResponseDto>> UpdateAsync(
        long id,
        UpdatePurchaseOrderDto dto,
        CancellationToken cancellationToken = default)
    {
        var res = await base.UpdateAsync(id, dto, cancellationToken);
        if (!res.Success) return res;

        await RecalcTotalsAsync(id, cancellationToken);
        var po = await _purchaseOrders.GetByIdAsync(id, cancellationToken);
        return po is null
            ? BaseResponse<PurchaseOrderResponseDto>.Fail("PurchaseOrder not found.")
            : BaseResponse<PurchaseOrderResponseDto>.Ok(Mapper.Map<PurchaseOrderResponseDto>(po), "Updated.");
    }

    private async Task RecalcTotalsAsync(long purchaseOrderId, CancellationToken ct)
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
