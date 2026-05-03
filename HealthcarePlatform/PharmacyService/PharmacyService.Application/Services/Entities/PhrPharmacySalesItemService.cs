using System.Data;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.DTOs.Stock;
using PharmacyService.Application.Services;
using PharmacyService.Application.Services.Extended;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;
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
    private readonly IRepository<PhrPharmacySale> _sales;
    private readonly IRepository<PhrMedicineBatch> _batches;
    private readonly IRepository<PhrStockLedger> _ledger;
    private readonly IPharmacyStockMovementService _stockMovement;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrPharmacySalesItemService(
        IRepository<PhrPharmacySalesItem> repository,
        IRepository<PhrPharmacySale> sales,
        IRepository<PhrMedicineBatch> batches,
        IRepository<PhrStockLedger> ledger,
        IPharmacyStockMovementService stockMovement,
        IPharmacyUnitOfWork uow,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePharmacySalesItemDto>? createValidator,
        IValidator<UpdatePharmacySalesItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrPharmacySalesItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _sales = sales;
        _batches = batches;
        _ledger = ledger;
        _stockMovement = stockMovement;
        _uow = uow;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<PharmacySalesItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<PharmacySalesItemResponseDto>> CreateAsync(
        CreatePharmacySalesItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantitySold <= 0)
            return BaseResponse<PharmacySalesItemResponseDto>.Fail("QuantitySold must be greater than zero.");

        var sale = await _sales.GetByIdAsync(dto.PharmacySalesId, cancellationToken);
        if (sale is null) return BaseResponse<PharmacySalesItemResponseDto>.Fail("Pharmacy sale not found.");
        if (Tenant.FacilityId is { } tf && sale.FacilityId is { } sf && sf != tf)
            return BaseResponse<PharmacySalesItemResponseDto>.Fail("Resource is not in the current facility scope.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                IReadOnlyList<StockFefoAllocation> allocations;
                if (dto.MedicineBatchId > 0)
                {
                    var b = await _batches.GetByIdAsync(dto.MedicineBatchId, ct);
                    if (b is null || b.IsDeleted)
                        return BaseResponse<PharmacySalesItemResponseDto>.Fail("Medicine batch not found.");
                    if (b.MedicineId != dto.MedicineId)
                        return BaseResponse<PharmacySalesItemResponseDto>.Fail("MedicineId does not match the selected batch.");
                    allocations = new[] { new StockFefoAllocation(b.Id, dto.QuantitySold) };
                }
                else
                {
                    var fefo = await _stockMovement.AllocateSaleFefoAsync(dto.MedicineId, dto.QuantitySold, ct);
                    if (!fefo.Success)
                        return BaseResponse<PharmacySalesItemResponseDto>.Fail(fefo.Message ?? "FEFO allocation failed.");
                    allocations = fefo.Data ?? Array.Empty<StockFefoAllocation>();
                }

                var entity = Mapper.Map<PhrPharmacySalesItem>(dto);
                entity.MedicineBatchId = allocations[0].MedicineBatchId;
                AuditHelper.ApplyCreate(entity, Tenant);
                entity.FacilityId = Tenant.FacilityId;

                await Repository.AddAsync(entity, ct);
                await Repository.SaveChangesAsync(ct);

                var saleDate = sale.SalesDate == default ? DateTime.UtcNow : sale.SalesDate;
                foreach (var a in allocations)
                {
                    var mv = await _stockMovement.ApplyMovementAsync(
                        StockLedgerTransactionType.SALE,
                        dto.PharmacySalesId,
                        entity.Id,
                        dto.MedicineId,
                        a.MedicineBatchId,
                        -a.Quantity,
                        saleDate,
                        dto.UnitPrice,
                        dto.Notes,
                        ct);
                    if (!mv.Success)
                        return BaseResponse<PharmacySalesItemResponseDto>.Fail(mv.Message ?? "Sale stock movement failed.");
                }

                return BaseResponse<PharmacySalesItemResponseDto>.Ok(Mapper.Map<PharmacySalesItemResponseDto>(entity), "Created.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Pharmacy sales item create failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<PharmacySalesItemResponseDto>.Fail("Create failed.");
        }
    }

    public override async Task<BaseResponse<PharmacySalesItemResponseDto>> UpdateAsync(
        long id,
        UpdatePharmacySalesItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.QuantitySold <= 0)
            return BaseResponse<PharmacySalesItemResponseDto>.Fail("QuantitySold must be greater than zero.");

        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<PharmacySalesItemResponseDto>.Fail("PharmacySalesItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<PharmacySalesItemResponseDto>.Fail("Resource is not in the current facility scope.");

        var sale = await _sales.GetByIdAsync(entity.PharmacySalesId, cancellationToken);
        if (sale is null) return BaseResponse<PharmacySalesItemResponseDto>.Fail("Pharmacy sale not found.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var revFail = await ReverseSaleLedgerForLineAsync(entity.PharmacySalesId, entity.Id, ct);
                if (revFail is not null)
                    return BaseResponse<PharmacySalesItemResponseDto>.Fail(revFail);

                IReadOnlyList<StockFefoAllocation> allocations;
                if (dto.MedicineBatchId > 0)
                {
                    var b = await _batches.GetByIdAsync(dto.MedicineBatchId, ct);
                    if (b is null || b.IsDeleted)
                        return BaseResponse<PharmacySalesItemResponseDto>.Fail("Medicine batch not found.");
                    if (b.MedicineId != dto.MedicineId)
                        return BaseResponse<PharmacySalesItemResponseDto>.Fail("MedicineId does not match the selected batch.");
                    allocations = new[] { new StockFefoAllocation(b.Id, dto.QuantitySold) };
                }
                else
                {
                    var fefo = await _stockMovement.AllocateSaleFefoAsync(dto.MedicineId, dto.QuantitySold, ct);
                    if (!fefo.Success)
                        return BaseResponse<PharmacySalesItemResponseDto>.Fail(fefo.Message ?? "FEFO allocation failed.");
                    allocations = fefo.Data ?? Array.Empty<StockFefoAllocation>();
                }

                Mapper.Map(dto, entity);
                entity.MedicineBatchId = allocations[0].MedicineBatchId;
                AuditHelper.ApplyUpdate(entity, Tenant);
                await Repository.UpdateAsync(entity, ct);
                await Repository.SaveChangesAsync(ct);

                var saleDate = sale.SalesDate == default ? DateTime.UtcNow : sale.SalesDate;
                foreach (var a in allocations)
                {
                    var mv = await _stockMovement.ApplyMovementAsync(
                        StockLedgerTransactionType.SALE,
                        entity.PharmacySalesId,
                        entity.Id,
                        dto.MedicineId,
                        a.MedicineBatchId,
                        -a.Quantity,
                        saleDate,
                        dto.UnitPrice,
                        dto.Notes,
                        ct);
                    if (!mv.Success)
                        return BaseResponse<PharmacySalesItemResponseDto>.Fail(mv.Message ?? "Sale stock movement failed.");
                }

                return BaseResponse<PharmacySalesItemResponseDto>.Ok(Mapper.Map<PharmacySalesItemResponseDto>(entity), "Updated.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Pharmacy sales item update failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<PharmacySalesItemResponseDto>.Fail("Update failed.");
        }
    }

    public override async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<object?>.Fail("PharmacySalesItem not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var revFail = await ReverseSaleLedgerForLineAsync(entity.PharmacySalesId, entity.Id, ct);
                if (revFail is not null)
                    return BaseResponse<object?>.Fail(revFail);

                entity.IsDeleted = true;
                entity.IsActive = false;
                AuditHelper.ApplyUpdate(entity, Tenant);
                await Repository.UpdateAsync(entity, ct);
                await Repository.SaveChangesAsync(ct);

                return BaseResponse<object?>.Ok(null, "Deleted.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Pharmacy sales item delete failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<object?>.Fail("Delete failed.");
        }
    }

    private async Task<string?> ReverseSaleLedgerForLineAsync(long pharmacySalesId, long salesItemId, CancellationToken ct)
    {
        var fid = Tenant.FacilityId;
        var rows = await _ledger.ListAsync(
            l => l.ReferenceId == pharmacySalesId
                && l.ReferenceLineId == salesItemId
                && l.TransactionType == StockLedgerTransactionType.SALE
                && l.TenantId == Tenant.TenantId
                && (fid == null || l.FacilityId == fid),
            ct);

        var sale = await _sales.GetByIdAsync(pharmacySalesId, ct);
        if (sale is null)
            return "Pharmacy sale not found.";

        var saleDate = sale.SalesDate == default ? DateTime.UtcNow : sale.SalesDate;

        foreach (var row in rows.OrderBy(r => r.Id))
        {
            var mv = await _stockMovement.ApplyMovementAsync(
                StockLedgerTransactionType.SALE,
                pharmacySalesId,
                salesItemId,
                row.MedicineId,
                row.MedicineBatchId,
                -row.QuantityDelta,
                saleDate,
                row.UnitCost,
                "Sale line reversal",
                ct);
            if (!mv.Success)
                return mv.Message ?? "Sale reversal failed.";
        }

        return null;
    }
}
