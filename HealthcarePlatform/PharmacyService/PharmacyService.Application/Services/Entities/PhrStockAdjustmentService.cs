using System.Data;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services;
using PharmacyService.Application.Services.Extended;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrStockAdjustmentService
{
    Task<BaseResponse<StockAdjustmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockAdjustmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentResponseDto>> CreateAsync(CreateStockAdjustmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentResponseDto>> UpdateAsync(long id, UpdateStockAdjustmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentResponseDto>> PostAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockAdjustmentService : PhrCrudServiceBase<PhrStockAdjustment, CreateStockAdjustmentDto, UpdateStockAdjustmentDto, StockAdjustmentResponseDto, PhrStockAdjustmentService>, IPhrStockAdjustmentService
{
    private readonly IRepository<PhrStockAdjustmentItem> _items;
    private readonly IRepository<PhrMedicineBatch> _batches;
    private readonly IPharmacyStockMovementService _stockMovement;
    private readonly IPharmacyUnitOfWork _uow;

    public PhrStockAdjustmentService(
        IRepository<PhrStockAdjustment> repository,
        IRepository<PhrStockAdjustmentItem> items,
        IRepository<PhrMedicineBatch> batches,
        IPharmacyStockMovementService stockMovement,
        IPharmacyUnitOfWork uow,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateStockAdjustmentDto>? createValidator,
        IValidator<UpdateStockAdjustmentDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrStockAdjustmentService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _items = items;
        _batches = batches;
        _stockMovement = stockMovement;
        _uow = uow;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockAdjustmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    protected override async Task OnBeforeCreateAsync(PhrStockAdjustment entity, CreateStockAdjustmentDto dto, CancellationToken cancellationToken)
    {
        entity.Status = PharmacyStockAdjustmentStatus.Draft;
        if (string.IsNullOrWhiteSpace(entity.AdjustmentNo))
            entity.AdjustmentNo = await NextAdjustmentNoAsync(cancellationToken);
    }

    public override async Task<BaseResponse<StockAdjustmentResponseDto>> UpdateAsync(
        long id,
        UpdateStockAdjustmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var existing = await Repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return BaseResponse<StockAdjustmentResponseDto>.Fail("PhrStockAdjustment not found.");
        if (existing.Status == PharmacyStockAdjustmentStatus.Posted)
            return BaseResponse<StockAdjustmentResponseDto>.Fail("Posted adjustments cannot be edited.");

        return await base.UpdateAsync(id, dto, cancellationToken);
    }

    public override async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<object?>.Fail("PhrStockAdjustment not found.");
        if (!IsEntityInFacilityScope(entity))
            return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");
        if (entity.Status == PharmacyStockAdjustmentStatus.Posted)
            return BaseResponse<object?>.Fail("Posted adjustments cannot be deleted.");

        var lines = await _items.ListAsync(i => i.StockAdjustmentId == id && !i.IsDeleted, cancellationToken);
        foreach (var line in lines)
        {
            line.IsDeleted = true;
            line.IsActive = false;
            AuditHelper.ApplyUpdate(line, Tenant);
            await _items.UpdateAsync(line, cancellationToken);
        }

        await _items.SaveChangesAsync(cancellationToken);
        return await base.DeleteAsync(id, cancellationToken);
    }

    public async Task<BaseResponse<StockAdjustmentResponseDto>> PostAsync(long id, CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && Tenant.FacilityId is null)
            return BaseResponse<StockAdjustmentResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        var adj = await Repository.GetByIdAsync(id, cancellationToken);
        if (adj is null)
            return BaseResponse<StockAdjustmentResponseDto>.Fail("PhrStockAdjustment not found.");
        if (!IsEntityInFacilityScope(adj))
            return BaseResponse<StockAdjustmentResponseDto>.Fail("Resource is not in the current facility scope.");
        if (adj.Status != PharmacyStockAdjustmentStatus.Draft)
            return BaseResponse<StockAdjustmentResponseDto>.Fail("Only draft adjustments can be posted.");

        var lines = await _items.ListAsync(i => i.StockAdjustmentId == id && !i.IsDeleted, cancellationToken);
        if (lines.Count == 0)
            return BaseResponse<StockAdjustmentResponseDto>.Fail("Add at least one line before posting.");

        foreach (var line in lines)
        {
            if (line.QuantityDelta == 0)
                return BaseResponse<StockAdjustmentResponseDto>.Fail("Each line must have a non-zero adjust quantity.");
        }

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var adj2 = await Repository.GetByIdAsync(id, ct);
                if (adj2 is null)
                    return BaseResponse<StockAdjustmentResponseDto>.Fail("PhrStockAdjustment not found.");
                if (adj2.Status != PharmacyStockAdjustmentStatus.Draft)
                    return BaseResponse<StockAdjustmentResponseDto>.Fail("Only draft adjustments can be posted.");

                var lines2 = await _items.ListAsync(i => i.StockAdjustmentId == id && !i.IsDeleted, ct);
                if (lines2.Count == 0)
                    return BaseResponse<StockAdjustmentResponseDto>.Fail("Add at least one line before posting.");

                var txnDate = adj2.AdjustmentOn == default ? DateTime.UtcNow : adj2.AdjustmentOn;
                foreach (var line in lines2.OrderBy(l => l.LineNum).ThenBy(l => l.Id))
                {
                    var batch = await _batches.GetByIdAsync(line.MedicineBatchId, ct);
                    if (batch is null || batch.IsDeleted)
                        return BaseResponse<StockAdjustmentResponseDto>.Fail("Medicine batch not found.");

                    var mv = await _stockMovement.ApplyMovementAsync(
                        StockLedgerTransactionType.ADJUSTMENT,
                        id,
                        line.Id,
                        batch.MedicineId,
                        line.MedicineBatchId,
                        line.QuantityDelta,
                        txnDate,
                        line.UnitCost,
                        line.Notes,
                        ct);
                    if (!mv.Success)
                        return BaseResponse<StockAdjustmentResponseDto>.Fail(mv.Message ?? "Stock adjustment movement failed.");
                }

                adj2.Status = PharmacyStockAdjustmentStatus.Posted;
                AuditHelper.ApplyUpdate(adj2, Tenant);
                await Repository.UpdateAsync(adj2, ct);
                await Repository.SaveChangesAsync(ct);

                return BaseResponse<StockAdjustmentResponseDto>.Ok(Mapper.Map<StockAdjustmentResponseDto>(adj2), "Posted.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Stock adjustment post failed tenant {TenantId}", Tenant.TenantId);
            return BaseResponse<StockAdjustmentResponseDto>.Fail("Post failed.");
        }
    }

    private async Task<string> NextAdjustmentNoAsync(CancellationToken cancellationToken)
    {
        var prefix = $"SA-{DateTime.UtcNow:yyyyMMdd}-";
        var sameDay = await Repository.ListAsync(
            a => a.AdjustmentNo.StartsWith(prefix) && !a.IsDeleted && a.TenantId == Tenant.TenantId,
            cancellationToken);
        var max = 0;
        foreach (var a in sameDay)
        {
            var no = a.AdjustmentNo;
            if (no.Length <= prefix.Length)
                continue;
            var suffix = no.Substring(prefix.Length);
            if (int.TryParse(suffix, out var n) && n > max)
                max = n;
        }

        return prefix + (max + 1).ToString("D4");
    }
}
