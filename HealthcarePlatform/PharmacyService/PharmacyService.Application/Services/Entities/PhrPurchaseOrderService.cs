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

    public override async Task<BaseResponse<PurchaseOrderResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _purchaseOrders.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<PurchaseOrderResponseDto>.Fail("PurchaseOrder not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<PurchaseOrderResponseDto>.Fail("Resource is not in the current facility scope.");

        var dto = Mapper.Map<PurchaseOrderResponseDto>(entity);
        var lines = await _items.ListAsync(x => x.PurchaseOrderId == id, cancellationToken);
        dto.Items = lines
            .OrderBy(x => x.LineNum)
            .Select(x => Mapper.Map<PurchaseOrderItemResponseDto>(x))
            .ToList();
        return BaseResponse<PurchaseOrderResponseDto>.Ok(dto);
    }

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

                if (dto.Items is null || dto.Items.Count == 0)
                    return BaseResponse<PurchaseOrderResponseDto>.Fail("At least one item is required.");

                dto.PurchaseOrderNo = poNo;

                var entity = Mapper.Map<PhrPurchaseOrder>(dto);
                AuditHelper.ApplyCreate(entity, Tenant);
                entity.FacilityId = Tenant.FacilityId;

                await _purchaseOrders.AddAsync(entity, ct);
                await _purchaseOrders.SaveChangesAsync(ct);

                await UpsertItemsAsync(entity.Id, dto.Items, ct);
                await RecalcTotalsAsync(entity.Id, ct);

                var fresh = await _purchaseOrders.GetByIdAsync(entity.Id, ct);
                if (fresh is null) return BaseResponse<PurchaseOrderResponseDto>.Fail("PurchaseOrder not found.");

                var outDto = Mapper.Map<PurchaseOrderResponseDto>(fresh);
                var outLines = await _items.ListAsync(x => x.PurchaseOrderId == entity.Id, ct);
                outDto.Items = outLines.OrderBy(x => x.LineNum).Select(x => Mapper.Map<PurchaseOrderItemResponseDto>(x)).ToList();
                return BaseResponse<PurchaseOrderResponseDto>.Ok(outDto, "Created.");
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PO create failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<PurchaseOrderResponseDto>.Fail("Create failed.");
        }
    }

    public override async Task<BaseResponse<PurchaseOrderResponseDto>> UpdateAsync(
        long id,
        UpdatePurchaseOrderDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return BaseResponse<PurchaseOrderResponseDto>.Fail("At least one item is required.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var entity = await _purchaseOrders.GetByIdAsync(id, ct);
                if (entity is null) return BaseResponse<PurchaseOrderResponseDto>.Fail("PurchaseOrder not found.");
                if (!IsEntityInFacilityScope(entity)) return BaseResponse<PurchaseOrderResponseDto>.Fail("Resource is not in the current facility scope.");

                var poNo = (dto.PurchaseOrderNo ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(poNo)) poNo = entity.PurchaseOrderNo;
                if (!string.Equals(poNo, entity.PurchaseOrderNo, StringComparison.OrdinalIgnoreCase))
                {
                    var dup = await _purchaseOrders.ListAsync(x => x.Id != id && x.PurchaseOrderNo == poNo, ct);
                    if (dup.Count > 0) return BaseResponse<PurchaseOrderResponseDto>.Fail("Duplicate PO number.");
                    entity.PurchaseOrderNo = poNo;
                }

                entity.SupplierName = dto.SupplierName;
                entity.OrderDate = dto.OrderDate;
                entity.ExpectedOn = dto.ExpectedOn;
                entity.StatusReferenceValueId = dto.StatusReferenceValueId;
                entity.Notes = dto.Notes;
                entity.DiscountAmount = dto.DiscountAmount;
                entity.GstPercent = dto.GstPercent;
                entity.OtherTaxAmount = dto.OtherTaxAmount;
                AuditHelper.ApplyUpdate(entity, Tenant);

                await _purchaseOrders.UpdateAsync(entity, ct);
                await _purchaseOrders.SaveChangesAsync(ct);

                await UpsertItemsAsync(id, dto.Items, ct);
                await RecalcTotalsAsync(id, ct);

                var fresh = await _purchaseOrders.GetByIdAsync(id, ct);
                if (fresh is null) return BaseResponse<PurchaseOrderResponseDto>.Fail("PurchaseOrder not found.");
                var outDto = Mapper.Map<PurchaseOrderResponseDto>(fresh);
                var outLines = await _items.ListAsync(x => x.PurchaseOrderId == id, ct);
                outDto.Items = outLines.OrderBy(x => x.LineNum).Select(x => Mapper.Map<PurchaseOrderItemResponseDto>(x)).ToList();
                return BaseResponse<PurchaseOrderResponseDto>.Ok(outDto, "Updated.");
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PO update failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<PurchaseOrderResponseDto>.Fail("Update failed.");
        }
    }

    private async Task UpsertItemsAsync(long purchaseOrderId, List<PurchaseOrderItemUpsertDto> items, CancellationToken ct)
    {
        var existing = await _items.ListAsync(x => x.PurchaseOrderId == purchaseOrderId, ct);
        var byId = existing.ToDictionary(x => x.Id, x => x);
        var keep = new HashSet<long>();

        var lineNum = 1;
        foreach (var it in items)
        {
            if (it.MedicineId <= 0) throw new InvalidOperationException("MedicineId is required.");
            if (it.QuantityOrdered <= 0) throw new InvalidOperationException("Quantity must be greater than zero.");
            if (it.UnitPrice <= 0) throw new InvalidOperationException("UnitPrice must be greater than zero.");

            if (it.Id is { } id && id > 0 && byId.TryGetValue(id, out var ent))
            {
                ent.LineNum = lineNum++;
                ent.MedicineId = it.MedicineId;
                ent.QuantityOrdered = it.QuantityOrdered;
                ent.PurchaseRate = it.UnitPrice;
                ent.LineTotal = it.QuantityOrdered * it.UnitPrice;
                ent.Notes = it.Notes;
                AuditHelper.ApplyUpdate(ent, Tenant);
                await _items.UpdateAsync(ent, ct);
                keep.Add(ent.Id);
            }
            else
            {
                var newEnt = new PhrPurchaseOrderItem
                {
                    PurchaseOrderId = purchaseOrderId,
                    LineNum = lineNum++,
                    MedicineId = it.MedicineId,
                    QuantityOrdered = it.QuantityOrdered,
                    PurchaseRate = it.UnitPrice,
                    LineTotal = it.QuantityOrdered * it.UnitPrice,
                    Notes = it.Notes,
                    IsActive = true
                };
                AuditHelper.ApplyCreate(newEnt, Tenant);
                newEnt.FacilityId = Tenant.FacilityId;
                await _items.AddAsync(newEnt, ct);
            }
        }

        foreach (var ent in existing)
        {
            if (keep.Contains(ent.Id)) continue;
            ent.IsDeleted = true;
            ent.IsActive = false;
            AuditHelper.ApplyUpdate(ent, Tenant);
            await _items.UpdateAsync(ent, ct);
        }

        await _items.SaveChangesAsync(ct);
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
