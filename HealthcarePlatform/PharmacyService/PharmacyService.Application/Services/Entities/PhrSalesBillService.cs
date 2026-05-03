using System.Data;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrSalesBillService
{
    Task<BaseResponse<SalesBillResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SalesBillResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<SalesBillResponseDto>> CreateAsync(CreateSalesBillDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SalesBillResponseDto>> UpdateAsync(long id, UpdateSalesBillDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<SalesBillResponseDto>> PostAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrSalesBillService : IPhrSalesBillService
{
    private readonly IRepository<PhrSalesBill> _bills;
    private readonly IRepository<PhrSalesBillItem> _items;
    private readonly IRepository<PhrMedicine> _medicines;
    private readonly IRepository<PhrMedicineBatch> _batches;
    private readonly IRepository<PhrCustomer> _customers;
    private readonly IPharmacyStockMovementService _stockMovement;
    private readonly IPharmacyUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly IValidator<CreateSalesBillDto>? _createValidator;
    private readonly IValidator<UpdateSalesBillDto>? _updateValidator;
    private readonly ILogger<PhrSalesBillService> _logger;

    private const bool RequiresFacilityId = true;

    public PhrSalesBillService(
        IRepository<PhrSalesBill> bills,
        IRepository<PhrSalesBillItem> items,
        IRepository<PhrMedicine> medicines,
        IRepository<PhrMedicineBatch> batches,
        IRepository<PhrCustomer> customers,
        IPharmacyStockMovementService stockMovement,
        IPharmacyUnitOfWork uow,
        IMapper mapper,
        ITenantContext tenant,
        IFacilityTenantValidator facilityValidator,
        IValidator<CreateSalesBillDto>? createValidator,
        IValidator<UpdateSalesBillDto>? updateValidator,
        ILogger<PhrSalesBillService> logger)
    {
        _bills = bills;
        _items = items;
        _medicines = medicines;
        _batches = batches;
        _customers = customers;
        _stockMovement = stockMovement;
        _uow = uow;
        _mapper = mapper;
        _tenant = tenant;
        _facilityValidator = facilityValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private bool IsEntityInFacilityScope(PhrSalesBill entity) =>
        !RequiresFacilityId || _tenant.FacilityId is null || entity.FacilityId == _tenant.FacilityId;

    public async Task<BaseResponse<SalesBillResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _bills.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<SalesBillResponseDto>.Fail("SalesBill not found.");
        if (!IsEntityInFacilityScope(entity))
            return BaseResponse<SalesBillResponseDto>.Fail("Resource is not in the current facility scope.");

        return BaseResponse<SalesBillResponseDto>.Ok(await MapWithItemsAsync(entity, cancellationToken));
    }

    public async Task<BaseResponse<PagedResponse<SalesBillResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && _tenant.FacilityId is null)
            return BaseResponse<PagedResponse<SalesBillResponseDto>>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        System.Linq.Expressions.Expression<Func<PhrSalesBill, bool>>? filter = null;
        if (_tenant.FacilityId is { } fid)
            filter = e => e.FacilityId == fid;

        var (list, total) = await _bills.GetPagedByFilterAsync(query.Page, query.PageSize, filter, cancellationToken);
        var dtoItems = list.Select(b => _mapper.Map<SalesBillResponseDto>(b)).ToList();

        return BaseResponse<PagedResponse<SalesBillResponseDto>>.Ok(new PagedResponse<SalesBillResponseDto>
        {
            Items = dtoItems,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        });
    }

    public async Task<BaseResponse<SalesBillResponseDto>> CreateAsync(
        CreateSalesBillDto dto,
        CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && _tenant.FacilityId is null)
            return BaseResponse<SalesBillResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        if (_tenant.FacilityId is long fId)
        {
            var ctx = await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, fId, cancellationToken);
            if (ctx is null)
                return BaseResponse<SalesBillResponseDto>.Fail(
                    "Facility is not valid for this tenant (shared enterprise hierarchy).");
        }

        if (_createValidator is not null)
        {
            var v = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!v.IsValid)
                return BaseResponse<SalesBillResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));
        }

        var partyErr = ValidateParty(dto.CustomerId, dto.PatientId);
        if (partyErr is not null) return BaseResponse<SalesBillResponseDto>.Fail(partyErr);

        var custErr = await ValidateCustomerAsync(dto.CustomerId, cancellationToken);
        if (custErr is not null) return BaseResponse<SalesBillResponseDto>.Fail(custErr);

        var medErr = await ValidateMedicineLinesAsync(dto.Items, cancellationToken);
        if (medErr is not null) return BaseResponse<SalesBillResponseDto>.Fail(medErr);

        var subPreview = PreviewSubtotal(dto.Items);
        var finErr = ValidateFinancials(subPreview, dto.DiscountAmount);
        if (finErr is not null) return BaseResponse<SalesBillResponseDto>.Fail(finErr);

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var bill = new PhrSalesBill
                {
                    BillNo = await NextBillNoAsync(ct),
                    CustomerId = dto.CustomerId,
                    PatientId = dto.PatientId,
                    SalesDate = dto.SalesDate.Date,
                    Status = PharmacySalesBillStatus.Draft,
                    Notes = dto.Notes,
                    IsActive = true
                };
                ApplyTotals(bill, subPreview, dto.DiscountAmount, dto.GstPercent, dto.OtherTaxAmount);
                AuditHelper.ApplyCreate(bill, _tenant);
                bill.FacilityId = _tenant.FacilityId;

                await _bills.AddAsync(bill, ct);
                await _bills.SaveChangesAsync(ct);

                await AddDraftLinesAsync(bill.Id, dto.Items, ct);

                var fresh = await _bills.GetByIdAsync(bill.Id, ct);
                if (fresh is null) return BaseResponse<SalesBillResponseDto>.Fail("SalesBill not found.");
                return BaseResponse<SalesBillResponseDto>.Ok(await MapWithItemsAsync(fresh, ct), "Created.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sales bill create failed tenant {TenantId}", _tenant.TenantId);
            return BaseResponse<SalesBillResponseDto>.Fail("Create failed.");
        }
    }

    public async Task<BaseResponse<SalesBillResponseDto>> UpdateAsync(
        long id,
        UpdateSalesBillDto dto,
        CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && _tenant.FacilityId is null)
            return BaseResponse<SalesBillResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        if (_updateValidator is not null)
        {
            var v = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!v.IsValid)
                return BaseResponse<SalesBillResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));
        }

        var existing = await _bills.GetByIdAsync(id, cancellationToken);
        if (existing is null) return BaseResponse<SalesBillResponseDto>.Fail("SalesBill not found.");
        if (!IsEntityInFacilityScope(existing))
            return BaseResponse<SalesBillResponseDto>.Fail("Resource is not in the current facility scope.");
        if (existing.Status != PharmacySalesBillStatus.Draft)
            return BaseResponse<SalesBillResponseDto>.Fail("Posted sales bills cannot be edited.");

        var partyErr = ValidateParty(dto.CustomerId, dto.PatientId);
        if (partyErr is not null) return BaseResponse<SalesBillResponseDto>.Fail(partyErr);

        var custErr = await ValidateCustomerAsync(dto.CustomerId, cancellationToken);
        if (custErr is not null) return BaseResponse<SalesBillResponseDto>.Fail(custErr);

        var medErr = await ValidateMedicineLinesAsync(dto.Items, cancellationToken);
        if (medErr is not null) return BaseResponse<SalesBillResponseDto>.Fail(medErr);

        var subPrev = PreviewSubtotal(dto.Items);
        var finErr2 = ValidateFinancials(subPrev, dto.DiscountAmount);
        if (finErr2 is not null) return BaseResponse<SalesBillResponseDto>.Fail(finErr2);

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var bill = await _bills.GetByIdAsync(id, ct);
                if (bill is null) return BaseResponse<SalesBillResponseDto>.Fail("SalesBill not found.");
                if (bill.Status != PharmacySalesBillStatus.Draft)
                    return BaseResponse<SalesBillResponseDto>.Fail("Posted sales bills cannot be edited.");

                bill.CustomerId = dto.CustomerId;
                bill.PatientId = dto.PatientId;
                bill.SalesDate = dto.SalesDate.Date;
                bill.Notes = dto.Notes;
                ApplyTotals(bill, subPrev, dto.DiscountAmount, dto.GstPercent, dto.OtherTaxAmount);
                AuditHelper.ApplyUpdate(bill, _tenant);
                await _bills.UpdateAsync(bill, ct);
                await _bills.SaveChangesAsync(ct);

                var oldLines = await _items.ListAsync(i => i.SalesBillId == id && !i.IsDeleted, ct);
                foreach (var ol in oldLines)
                {
                    ol.IsDeleted = true;
                    ol.IsActive = false;
                    AuditHelper.ApplyUpdate(ol, _tenant);
                    await _items.UpdateAsync(ol, ct);
                }

                await _items.SaveChangesAsync(ct);
                await AddDraftLinesAsync(bill.Id, dto.Items, ct);

                var fresh = await _bills.GetByIdAsync(id, ct);
                if (fresh is null) return BaseResponse<SalesBillResponseDto>.Fail("SalesBill not found.");
                return BaseResponse<SalesBillResponseDto>.Ok(await MapWithItemsAsync(fresh, ct), "Updated.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sales bill update failed tenant {TenantId}", _tenant.TenantId);
            return BaseResponse<SalesBillResponseDto>.Fail("Update failed.");
        }
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _bills.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<object?>.Fail("SalesBill not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");
        if (entity.Status == PharmacySalesBillStatus.Posted)
            return BaseResponse<object?>.Fail("Posted sales bills cannot be deleted.");

        var lines = await _items.ListAsync(i => i.SalesBillId == id && !i.IsDeleted, cancellationToken);
        foreach (var line in lines)
        {
            line.IsDeleted = true;
            line.IsActive = false;
            AuditHelper.ApplyUpdate(line, _tenant);
            await _items.UpdateAsync(line, cancellationToken);
        }

        await _items.SaveChangesAsync(cancellationToken);
        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, _tenant);
        await _bills.UpdateAsync(entity, cancellationToken);
        await _bills.SaveChangesAsync(cancellationToken);
        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    public async Task<BaseResponse<SalesBillResponseDto>> PostAsync(long id, CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && _tenant.FacilityId is null)
            return BaseResponse<SalesBillResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        var bill = await _bills.GetByIdAsync(id, cancellationToken);
        if (bill is null) return BaseResponse<SalesBillResponseDto>.Fail("SalesBill not found.");
        if (!IsEntityInFacilityScope(bill))
            return BaseResponse<SalesBillResponseDto>.Fail("Resource is not in the current facility scope.");
        if (bill.Status != PharmacySalesBillStatus.Draft)
            return BaseResponse<SalesBillResponseDto>.Fail("Only draft sales bills can be posted.");

        var partyErr = ValidateParty(bill.CustomerId, bill.PatientId);
        if (partyErr is not null) return BaseResponse<SalesBillResponseDto>.Fail(partyErr);

        var draftLines = (await _items.ListAsync(i => i.SalesBillId == id && !i.IsDeleted, cancellationToken))
            .OrderBy(i => i.LineNum).ThenBy(i => i.Id).ToList();
        if (draftLines.Count == 0)
            return BaseResponse<SalesBillResponseDto>.Fail("At least one item is required.");

        foreach (var d in draftLines)
        {
            if (d.MedicineBatchId is not null)
                return BaseResponse<SalesBillResponseDto>.Fail("Draft lines must not specify a batch; use Post to apply FEFO.");
        }

        var snapshots = draftLines.Select(d => (d.MedicineId, d.Quantity, d.UnitPrice)).ToList();

        foreach (var s in snapshots)
        {
            var alloc = await _stockMovement.AllocateSaleFefoAsync(s.MedicineId, s.Quantity, cancellationToken);
            if (!alloc.Success)
                return BaseResponse<SalesBillResponseDto>.Fail(alloc.Message ?? "Insufficient stock for sale (FEFO).");
        }

        var subPreview = snapshots.Sum(s => Math.Round(s.Quantity * s.UnitPrice, 4, MidpointRounding.AwayFromZero));
        var finPost = ValidateFinancials(subPreview, bill.DiscountAmount);
        if (finPost is not null) return BaseResponse<SalesBillResponseDto>.Fail(finPost);

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var b2 = await _bills.GetByIdAsync(id, ct);
                if (b2 is null) return BaseResponse<SalesBillResponseDto>.Fail("SalesBill not found.");
                if (b2.Status != PharmacySalesBillStatus.Draft)
                    return BaseResponse<SalesBillResponseDto>.Fail("Only draft sales bills can be posted.");

                var toRemove = (await _items.ListAsync(i => i.SalesBillId == id && !i.IsDeleted, ct)).ToList();
                foreach (var ol in toRemove)
                {
                    ol.IsDeleted = true;
                    ol.IsActive = false;
                    AuditHelper.ApplyUpdate(ol, _tenant);
                    await _items.UpdateAsync(ol, ct);
                }

                await _items.SaveChangesAsync(ct);

                var saleDate = b2.SalesDate == default ? DateTime.UtcNow : b2.SalesDate;
                var lineNum = 1;
                foreach (var s in snapshots)
                {
                    var fefo = await _stockMovement.AllocateSaleFefoAsync(s.MedicineId, s.Quantity, ct);
                    if (!fefo.Success || fefo.Data is null)
                        return BaseResponse<SalesBillResponseDto>.Fail(fefo.Message ?? "FEFO allocation failed.");

                    foreach (var a in fefo.Data)
                    {
                        var lineTotal = Math.Round(a.Quantity * s.UnitPrice, 4, MidpointRounding.AwayFromZero);
                        var line = new PhrSalesBillItem
                        {
                            SalesBillId = id,
                            LineNum = lineNum++,
                            MedicineId = s.MedicineId,
                            MedicineBatchId = a.MedicineBatchId,
                            Quantity = a.Quantity,
                            UnitPrice = s.UnitPrice,
                            LineTotal = lineTotal,
                            IsActive = true
                        };
                        AuditHelper.ApplyCreate(line, _tenant);
                        line.FacilityId = _tenant.FacilityId;
                        await _items.AddAsync(line, ct);
                        await _items.SaveChangesAsync(ct);

                        var mv = await _stockMovement.ApplyMovementAsync(
                            StockLedgerTransactionType.SALE,
                            id,
                            line.Id,
                            s.MedicineId,
                            a.MedicineBatchId,
                            -a.Quantity,
                            saleDate,
                            s.UnitPrice,
                            $"Sales bill {b2.BillNo}",
                            ct);
                        if (!mv.Success)
                            return BaseResponse<SalesBillResponseDto>.Fail(mv.Message ?? "Stock deduction failed.");
                    }
                }

                var postedLines = await _items.ListAsync(i => i.SalesBillId == id && !i.IsDeleted, ct);
                var lineSum = postedLines.Sum(x => x.LineTotal);
                ApplyTotals(b2, lineSum, b2.DiscountAmount, b2.GstPercent, b2.OtherTaxAmount);

                b2.Status = PharmacySalesBillStatus.Posted;
                AuditHelper.ApplyUpdate(b2, _tenant);
                await _bills.UpdateAsync(b2, ct);
                await _bills.SaveChangesAsync(ct);

                return BaseResponse<SalesBillResponseDto>.Ok(await MapWithItemsAsync(b2, ct), "Posted.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sales bill post failed tenant {TenantId}", _tenant.TenantId);
            return BaseResponse<SalesBillResponseDto>.Fail("Post failed.");
        }
    }

    private static string? ValidateParty(long? customerId, long? patientId)
    {
        var hasC = customerId is > 0;
        var hasP = patientId is > 0;
        if (hasC == hasP)
            return "Specify exactly one of customer or registered patient.";
        return null;
    }

    private async Task<string?> ValidateCustomerAsync(long? customerId, CancellationToken ct)
    {
        if (customerId is not > 0) return null;
        var c = await _customers.GetByIdAsync(customerId.Value, ct);
        if (c is null || c.IsDeleted) return "Customer not found.";
        return null;
    }

    private async Task<string?> ValidateMedicineLinesAsync(List<SalesBillLineInputDto> lines, CancellationToken ct)
    {
        var ids = lines.Select(l => l.MedicineId).Distinct().ToList();
        var meds = await _medicines.ListAsync(m => ids.Contains(m.Id) && !m.IsDeleted, ct);
        if (meds.Count != ids.Count) return "One or more medicines were not found.";
        return null;
    }

    private static decimal PreviewSubtotal(List<SalesBillLineInputDto> inputs)
    {
        decimal sub = 0m;
        foreach (var inp in inputs)
            sub += Math.Round(inp.Quantity * inp.UnitPrice, 4, MidpointRounding.AwayFromZero);
        return sub;
    }

    private static string? ValidateFinancials(decimal subtotal, decimal discountAmount)
    {
        if (discountAmount < 0) return "Discount cannot be negative.";
        if (discountAmount > subtotal) return "Discount cannot exceed subtotal.";
        return null;
    }

    private static void ApplyTotals(
        PhrSalesBill bill,
        decimal lineSubTotal,
        decimal discountAmount,
        decimal gstPercent,
        decimal otherTaxAmount)
    {
        bill.SubTotal = lineSubTotal;
        bill.DiscountAmount = discountAmount;
        bill.GstPercent = gstPercent;
        bill.OtherTaxAmount = otherTaxAmount;

        var taxable = Math.Max(0m, lineSubTotal - discountAmount);
        bill.GstAmount = Math.Round(taxable * (gstPercent / 100m), 4, MidpointRounding.AwayFromZero);
        bill.NetAmount = Math.Round(lineSubTotal - discountAmount + bill.GstAmount + otherTaxAmount, 4, MidpointRounding.AwayFromZero);
    }

    private async Task AddDraftLinesAsync(long salesBillId, List<SalesBillLineInputDto> inputs, CancellationToken ct)
    {
        var lineNum = 1;
        foreach (var inp in inputs)
        {
            var lineTotal = Math.Round(inp.Quantity * inp.UnitPrice, 4, MidpointRounding.AwayFromZero);
            var line = new PhrSalesBillItem
            {
                SalesBillId = salesBillId,
                LineNum = lineNum++,
                MedicineId = inp.MedicineId,
                MedicineBatchId = null,
                Quantity = inp.Quantity,
                UnitPrice = inp.UnitPrice,
                LineTotal = lineTotal,
                IsActive = true
            };
            AuditHelper.ApplyCreate(line, _tenant);
            line.FacilityId = _tenant.FacilityId;
            await _items.AddAsync(line, ct);
        }

        await _items.SaveChangesAsync(ct);
    }

    private async Task<SalesBillResponseDto> MapWithItemsAsync(PhrSalesBill bill, CancellationToken ct)
    {
        var dto = _mapper.Map<SalesBillResponseDto>(bill);
        var lines = await _items.ListAsync(i => i.SalesBillId == bill.Id && !i.IsDeleted, ct);
        var medIds = lines.Select(l => l.MedicineId).Distinct().ToList();
        var batchIds = lines.Select(l => l.MedicineBatchId).Where(b => b is not null).Select(b => b!.Value).Distinct().ToList();
        var meds = await _medicines.ListAsync(m => medIds.Contains(m.Id) && !m.IsDeleted, ct);
        var medById = meds.ToDictionary(m => m.Id, m => m);
        var batches = batchIds.Count == 0
            ? new List<PhrMedicineBatch>()
            : await _batches.ListAsync(b => batchIds.Contains(b.Id) && !b.IsDeleted, ct);
        var batchById = batches.ToDictionary(b => b.Id, b => b);

        dto.Items = lines.OrderBy(l => l.LineNum).Select(l =>
        {
            var item = _mapper.Map<SalesBillItemResponseDto>(l);
            item.MedicineName = medById.TryGetValue(l.MedicineId, out var m) ? m.MedicineName : "?";
            if (l.MedicineBatchId is { } bid && batchById.TryGetValue(bid, out var bb))
                item.BatchNo = bb.BatchNo;
            return item;
        }).ToList();
        return dto;
    }

    private async Task<string> NextBillNoAsync(CancellationToken ct)
    {
        var prefix = $"SB-{DateTime.UtcNow:yyyyMMdd}-";
        var sameDay = await _bills.ListAsync(
            b => b.BillNo.StartsWith(prefix) && !b.IsDeleted && b.TenantId == _tenant.TenantId,
            ct);
        var max = 0;
        foreach (var b in sameDay)
        {
            if (b.BillNo.Length <= prefix.Length) continue;
            var suffix = b.BillNo.Substring(prefix.Length);
            if (int.TryParse(suffix, out var n) && n > max) max = n;
        }

        return prefix + (max + 1).ToString("D4");
    }
}
