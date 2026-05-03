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

public interface IPhrPurchaseBillService
{
    Task<BaseResponse<PurchaseBillResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PurchaseBillResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PurchaseBillResponseDto>> CreateAsync(CreatePurchaseBillDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PurchaseBillResponseDto>> UpdateAsync(long id, UpdatePurchaseBillDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PurchaseBillResponseDto>> PostAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrPurchaseBillService : IPhrPurchaseBillService
{
    private readonly IRepository<PhrPurchaseBill> _bills;
    private readonly IRepository<PhrPurchaseBillItem> _items;
    private readonly IRepository<PhrGoodsReceipt> _goodsReceipts;
    private readonly IRepository<PhrGoodsReceiptItem> _grnItems;
    private readonly IRepository<PhrPurchaseOrder> _purchaseOrders;
    private readonly IRepository<PhrMedicine> _medicines;
    private readonly IPharmacyUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly IValidator<CreatePurchaseBillDto>? _createValidator;
    private readonly IValidator<UpdatePurchaseBillDto>? _updateValidator;
    private readonly ILogger<PhrPurchaseBillService> _logger;

    private const bool RequiresFacilityId = true;

    public PhrPurchaseBillService(
        IRepository<PhrPurchaseBill> bills,
        IRepository<PhrPurchaseBillItem> items,
        IRepository<PhrGoodsReceipt> goodsReceipts,
        IRepository<PhrGoodsReceiptItem> grnItems,
        IRepository<PhrPurchaseOrder> purchaseOrders,
        IRepository<PhrMedicine> medicines,
        IPharmacyUnitOfWork uow,
        IMapper mapper,
        ITenantContext tenant,
        IFacilityTenantValidator facilityValidator,
        IValidator<CreatePurchaseBillDto>? createValidator,
        IValidator<UpdatePurchaseBillDto>? updateValidator,
        ILogger<PhrPurchaseBillService> logger)
    {
        _bills = bills;
        _items = items;
        _goodsReceipts = goodsReceipts;
        _grnItems = grnItems;
        _purchaseOrders = purchaseOrders;
        _medicines = medicines;
        _uow = uow;
        _mapper = mapper;
        _tenant = tenant;
        _facilityValidator = facilityValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private bool IsEntityInFacilityScope(PhrPurchaseBill entity) =>
        !RequiresFacilityId || _tenant.FacilityId is null || entity.FacilityId == _tenant.FacilityId;

    public async Task<BaseResponse<PurchaseBillResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _bills.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<PurchaseBillResponseDto>.Fail("PurchaseBill not found.");
        if (!IsEntityInFacilityScope(entity))
            return BaseResponse<PurchaseBillResponseDto>.Fail("Resource is not in the current facility scope.");

        return BaseResponse<PurchaseBillResponseDto>.Ok(await MapWithItemsAsync(entity, cancellationToken));
    }

    public async Task<BaseResponse<PagedResponse<PurchaseBillResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && _tenant.FacilityId is null)
            return BaseResponse<PagedResponse<PurchaseBillResponseDto>>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        System.Linq.Expressions.Expression<Func<PhrPurchaseBill, bool>>? filter = null;
        if (_tenant.FacilityId is { } fid)
            filter = e => e.FacilityId == fid;

        var (list, total) = await _bills.GetPagedByFilterAsync(query.Page, query.PageSize, filter, cancellationToken);
        var dtoItems = new List<PurchaseBillResponseDto>();
        foreach (var b in list)
            dtoItems.Add(_mapper.Map<PurchaseBillResponseDto>(b));

        return BaseResponse<PagedResponse<PurchaseBillResponseDto>>.Ok(new PagedResponse<PurchaseBillResponseDto>
        {
            Items = dtoItems,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        });
    }

    public async Task<BaseResponse<PurchaseBillResponseDto>> CreateAsync(
        CreatePurchaseBillDto dto,
        CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && _tenant.FacilityId is null)
            return BaseResponse<PurchaseBillResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        if (_tenant.FacilityId is long fId)
        {
            var ctx = await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, fId, cancellationToken);
            if (ctx is null)
                return BaseResponse<PurchaseBillResponseDto>.Fail(
                    "Facility is not valid for this tenant (shared enterprise hierarchy).");
        }

        if (_createValidator is not null)
        {
            var v = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!v.IsValid)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));
        }

        var grn = await _goodsReceipts.GetByIdAsync(dto.GoodsReceiptId, cancellationToken);
        if (grn is null || grn.IsDeleted)
            return BaseResponse<PurchaseBillResponseDto>.Fail("Goods receipt not found.");
        if (!IsGrnInFacility(grn))
            return BaseResponse<PurchaseBillResponseDto>.Fail("Resource is not in the current facility scope.");
        if (grn.SupplierId is null || grn.SupplierId <= 0)
            return BaseResponse<PurchaseBillResponseDto>.Fail("Goods receipt must have a supplier to bill.");
        if (grn.SupplierId != dto.SupplierId)
            return BaseResponse<PurchaseBillResponseDto>.Fail("Supplier must match the goods receipt.");

        if (dto.SourceMode == PharmacyPurchaseBillSourceMode.PurchaseOrderLinked)
        {
            if (dto.PurchaseOrderId is null || dto.PurchaseOrderId <= 0)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Purchase order is required for PO-linked mode.");
            if (grn.PurchaseOrderId != dto.PurchaseOrderId)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Goods receipt is not linked to the selected purchase order.");
        }
        else
        {
            if (dto.PurchaseOrderId is not null)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Purchase order must be empty for direct GRN mode.");
            if (grn.PurchaseOrderId is not null)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Select a goods receipt without a purchase order for direct mode.");
        }

        if (await InvoiceExistsAsync(dto.SupplierId, dto.InvoiceNo, excludeBillId: null, cancellationToken))
            return BaseResponse<PurchaseBillResponseDto>.Fail("Invoice number must be unique per supplier.");

        var grnLines = (await _grnItems.ListAsync(i => i.GoodsReceiptId == grn.Id && !i.IsDeleted, cancellationToken))
            .OrderBy(i => i.LineNum).ThenBy(i => i.Id).ToList();
        if (grnLines.Count == 0)
            return BaseResponse<PurchaseBillResponseDto>.Fail("Goods receipt has no lines.");

        List<PurchaseBillLineInputDto> lineInputs;
        if (dto.Items is null || dto.Items.Count == 0)
        {
            lineInputs = grnLines.Select(l => new PurchaseBillLineInputDto
            {
                GoodsReceiptItemId = l.Id,
                Quantity = l.QuantityReceived,
                Rate = l.PurchaseRate ?? 0m
            }).ToList();
        }
        else
        {
            var err = ValidateLineInputsAgainstGrn(grnLines, dto.Items);
            if (err is not null) return BaseResponse<PurchaseBillResponseDto>.Fail(err);
            lineInputs = dto.Items;
        }

        var lineErr = await ValidateQuantitiesAgainstGrn(grnLines, lineInputs, excludePurchaseBillId: null, cancellationToken);
        if (lineErr is not null) return BaseResponse<PurchaseBillResponseDto>.Fail(lineErr);

        decimal discountAmt;
        decimal gstPct;
        decimal otherTaxAmt;
        if (dto.SourceMode == PharmacyPurchaseBillSourceMode.PurchaseOrderLinked)
        {
            var po = await _purchaseOrders.GetByIdAsync(dto.PurchaseOrderId!.Value, cancellationToken);
            if (po is null || po.IsDeleted)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Purchase order not found.");
            if (!IsPoInFacilityScope(po))
                return BaseResponse<PurchaseBillResponseDto>.Fail("Resource is not in the current facility scope.");
            discountAmt = po.DiscountAmount;
            gstPct = po.GstPercent;
            otherTaxAmt = po.OtherTaxAmount;
        }
        else
        {
            discountAmt = dto.DiscountAmount;
            gstPct = dto.GstPercent;
            otherTaxAmt = dto.OtherTaxAmount;
        }

        var subPreview = PreviewSubtotal(grnLines, lineInputs);
        var finErr = ValidateFinancials(subPreview, discountAmt);
        if (finErr is not null) return BaseResponse<PurchaseBillResponseDto>.Fail(finErr);

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                if (await InvoiceExistsAsync(dto.SupplierId, dto.InvoiceNo, excludeBillId: null, ct))
                    return BaseResponse<PurchaseBillResponseDto>.Fail("Invoice number must be unique per supplier.");

                var bill = new PhrPurchaseBill
                {
                    BillNo = await NextBillNoAsync(ct),
                    InvoiceNo = dto.InvoiceNo.Trim(),
                    InvoiceDate = dto.InvoiceDate.Date,
                    SupplierId = dto.SupplierId,
                    PurchaseOrderId = dto.PurchaseOrderId,
                    GoodsReceiptId = dto.GoodsReceiptId,
                    SourceMode = dto.SourceMode,
                    Status = PharmacyPurchaseBillStatus.Draft,
                    Notes = dto.Notes,
                    IsActive = true
                };

                ApplyTotals(bill, lineInputs, grnLines, discountAmt, gstPct, otherTaxAmt);
                AuditHelper.ApplyCreate(bill, _tenant);
                bill.FacilityId = _tenant.FacilityId;

                await _bills.AddAsync(bill, ct);
                await _bills.SaveChangesAsync(ct);

                await AddBillLinesAsync(bill.Id, grn.Id, grnLines, lineInputs, ct);

                var fresh = await _bills.GetByIdAsync(bill.Id, ct);
                if (fresh is null) return BaseResponse<PurchaseBillResponseDto>.Fail("PurchaseBill not found.");
                return BaseResponse<PurchaseBillResponseDto>.Ok(await MapWithItemsAsync(fresh, ct), "Created.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Purchase bill create failed tenant {TenantId}", _tenant.TenantId);
            return BaseResponse<PurchaseBillResponseDto>.Fail("Create failed.");
        }
    }

    public async Task<BaseResponse<PurchaseBillResponseDto>> UpdateAsync(
        long id,
        UpdatePurchaseBillDto dto,
        CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && _tenant.FacilityId is null)
            return BaseResponse<PurchaseBillResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        if (_updateValidator is not null)
        {
            var v = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!v.IsValid)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));
        }

        var existing = await _bills.GetByIdAsync(id, cancellationToken);
        if (existing is null) return BaseResponse<PurchaseBillResponseDto>.Fail("PurchaseBill not found.");
        if (!IsEntityInFacilityScope(existing))
            return BaseResponse<PurchaseBillResponseDto>.Fail("Resource is not in the current facility scope.");
        if (existing.Status != PharmacyPurchaseBillStatus.Draft)
            return BaseResponse<PurchaseBillResponseDto>.Fail("Posted bills cannot be edited.");

        if (dto.Items is null || dto.Items.Count == 0)
            return BaseResponse<PurchaseBillResponseDto>.Fail("At least one item is required.");

        if (await InvoiceExistsAsync(existing.SupplierId, dto.InvoiceNo, excludeBillId: id, cancellationToken))
            return BaseResponse<PurchaseBillResponseDto>.Fail("Invoice number must be unique per supplier.");

        var grnLines = (await _grnItems.ListAsync(i => i.GoodsReceiptId == existing.GoodsReceiptId && !i.IsDeleted, cancellationToken))
            .OrderBy(i => i.LineNum).ThenBy(i => i.Id).ToList();
        var err = ValidateLineInputsAgainstGrn(grnLines, dto.Items);
        if (err is not null) return BaseResponse<PurchaseBillResponseDto>.Fail(err);

        var qErr = await ValidateQuantitiesAgainstGrn(grnLines, dto.Items, excludePurchaseBillId: id, cancellationToken);
        if (qErr is not null) return BaseResponse<PurchaseBillResponseDto>.Fail(qErr);

        decimal discountAmt;
        decimal gstPct;
        decimal otherTaxAmt;
        if (existing.SourceMode == PharmacyPurchaseBillSourceMode.PurchaseOrderLinked)
        {
            if (existing.PurchaseOrderId is not { } poIdUpd)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Purchase order is required for PO-linked bills.");
            var poUpd = await _purchaseOrders.GetByIdAsync(poIdUpd, cancellationToken);
            if (poUpd is null || poUpd.IsDeleted)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Purchase order not found.");
            if (!IsPoInFacilityScope(poUpd))
                return BaseResponse<PurchaseBillResponseDto>.Fail("Resource is not in the current facility scope.");
            discountAmt = poUpd.DiscountAmount;
            gstPct = poUpd.GstPercent;
            otherTaxAmt = poUpd.OtherTaxAmount;
        }
        else
        {
            discountAmt = dto.DiscountAmount;
            gstPct = dto.GstPercent;
            otherTaxAmt = dto.OtherTaxAmount;
        }

        var subPrev = PreviewSubtotal(grnLines, dto.Items);
        var finErr2 = ValidateFinancials(subPrev, discountAmt);
        if (finErr2 is not null) return BaseResponse<PurchaseBillResponseDto>.Fail(finErr2);

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var bill = await _bills.GetByIdAsync(id, ct);
                if (bill is null) return BaseResponse<PurchaseBillResponseDto>.Fail("PurchaseBill not found.");
                if (bill.Status != PharmacyPurchaseBillStatus.Draft)
                    return BaseResponse<PurchaseBillResponseDto>.Fail("Posted bills cannot be edited.");
                if (await InvoiceExistsAsync(bill.SupplierId, dto.InvoiceNo, excludeBillId: id, ct))
                    return BaseResponse<PurchaseBillResponseDto>.Fail("Invoice number must be unique per supplier.");

                bill.InvoiceNo = dto.InvoiceNo.Trim();
                bill.InvoiceDate = dto.InvoiceDate.Date;
                bill.Notes = dto.Notes;
                ApplyTotals(bill, dto.Items, grnLines, discountAmt, gstPct, otherTaxAmt);
                AuditHelper.ApplyUpdate(bill, _tenant);
                await _bills.UpdateAsync(bill, ct);
                await _bills.SaveChangesAsync(ct);

                var oldLines = await _items.ListAsync(i => i.PurchaseBillId == id && !i.IsDeleted, ct);
                foreach (var ol in oldLines)
                {
                    ol.IsDeleted = true;
                    ol.IsActive = false;
                    AuditHelper.ApplyUpdate(ol, _tenant);
                    await _items.UpdateAsync(ol, ct);
                }

                await _items.SaveChangesAsync(ct);
                await AddBillLinesAsync(bill.Id, bill.GoodsReceiptId, grnLines, dto.Items, ct);

                var fresh = await _bills.GetByIdAsync(id, ct);
                if (fresh is null) return BaseResponse<PurchaseBillResponseDto>.Fail("PurchaseBill not found.");
                return BaseResponse<PurchaseBillResponseDto>.Ok(await MapWithItemsAsync(fresh, ct), "Updated.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Purchase bill update failed tenant {TenantId}", _tenant.TenantId);
            return BaseResponse<PurchaseBillResponseDto>.Fail("Update failed.");
        }
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _bills.GetByIdAsync(id, cancellationToken);
        if (entity is null) return BaseResponse<object?>.Fail("PurchaseBill not found.");
        if (!IsEntityInFacilityScope(entity)) return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");
        if (entity.Status == PharmacyPurchaseBillStatus.Posted)
            return BaseResponse<object?>.Fail("Posted bills cannot be deleted.");

        var lines = await _items.ListAsync(i => i.PurchaseBillId == id && !i.IsDeleted, cancellationToken);
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

    public async Task<BaseResponse<PurchaseBillResponseDto>> PostAsync(long id, CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && _tenant.FacilityId is null)
            return BaseResponse<PurchaseBillResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        var bill = await _bills.GetByIdAsync(id, cancellationToken);
        if (bill is null) return BaseResponse<PurchaseBillResponseDto>.Fail("PurchaseBill not found.");
        if (!IsEntityInFacilityScope(bill))
            return BaseResponse<PurchaseBillResponseDto>.Fail("Resource is not in the current facility scope.");
        if (bill.Status != PharmacyPurchaseBillStatus.Draft)
            return BaseResponse<PurchaseBillResponseDto>.Fail("Only draft bills can be posted.");

        var lines = await _items.ListAsync(i => i.PurchaseBillId == id && !i.IsDeleted, cancellationToken);
        if (lines.Count == 0)
            return BaseResponse<PurchaseBillResponseDto>.Fail("At least one item is required.");

        if (await InvoiceExistsAsync(bill.SupplierId, bill.InvoiceNo, excludeBillId: id, cancellationToken))
            return BaseResponse<PurchaseBillResponseDto>.Fail("Invoice number must be unique per supplier.");

        var grnLines = (await _grnItems.ListAsync(i => i.GoodsReceiptId == bill.GoodsReceiptId && !i.IsDeleted, cancellationToken))
            .ToDictionary(i => i.Id, i => i);
        var inputs = lines.OrderBy(l => l.LineNum).Select(l => new PurchaseBillLineInputDto
        {
            GoodsReceiptItemId = l.GoodsReceiptItemId,
            Quantity = l.Quantity,
            Rate = l.Rate
        }).ToList();
        var qErr = await ValidateQuantitiesAgainstGrn(grnLines.Values.ToList(), inputs, excludePurchaseBillId: id, cancellationToken);
        if (qErr is not null) return BaseResponse<PurchaseBillResponseDto>.Fail(qErr);

        decimal postDisc;
        decimal postGst;
        decimal postOther;
        if (bill.SourceMode == PharmacyPurchaseBillSourceMode.PurchaseOrderLinked)
        {
            if (bill.PurchaseOrderId is not { } poIdPost)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Purchase order is required for PO-linked bills.");
            var poPost = await _purchaseOrders.GetByIdAsync(poIdPost, cancellationToken);
            if (poPost is null || poPost.IsDeleted)
                return BaseResponse<PurchaseBillResponseDto>.Fail("Purchase order not found.");
            if (!IsPoInFacilityScope(poPost))
                return BaseResponse<PurchaseBillResponseDto>.Fail("Resource is not in the current facility scope.");
            postDisc = poPost.DiscountAmount;
            postGst = poPost.GstPercent;
            postOther = poPost.OtherTaxAmount;
        }
        else
        {
            postDisc = bill.DiscountAmount;
            postGst = bill.GstPercent;
            postOther = bill.OtherTaxAmount;
        }

        var subPost = PreviewSubtotal(grnLines.Values.ToList(), inputs);
        var finPost = ValidateFinancials(subPost, postDisc);
        if (finPost is not null) return BaseResponse<PurchaseBillResponseDto>.Fail(finPost);

        try
        {
            return await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var b2 = await _bills.GetByIdAsync(id, ct);
                if (b2 is null) return BaseResponse<PurchaseBillResponseDto>.Fail("PurchaseBill not found.");
                if (b2.Status != PharmacyPurchaseBillStatus.Draft)
                    return BaseResponse<PurchaseBillResponseDto>.Fail("Only draft bills can be posted.");
                if (await InvoiceExistsAsync(b2.SupplierId, b2.InvoiceNo, excludeBillId: id, ct))
                    return BaseResponse<PurchaseBillResponseDto>.Fail("Invoice number must be unique per supplier.");

                var glPost = (await _grnItems.ListAsync(i => i.GoodsReceiptId == b2.GoodsReceiptId && !i.IsDeleted, ct))
                    .OrderBy(i => i.LineNum).ThenBy(i => i.Id).ToList();
                var linesPost = await _items.ListAsync(i => i.PurchaseBillId == id && !i.IsDeleted, ct);
                var inputsPost = linesPost.OrderBy(l => l.LineNum).Select(l => new PurchaseBillLineInputDto
                {
                    GoodsReceiptItemId = l.GoodsReceiptItemId,
                    Quantity = l.Quantity,
                    Rate = l.Rate
                }).ToList();
                ApplyTotals(b2, inputsPost, glPost, postDisc, postGst, postOther);

                b2.Status = PharmacyPurchaseBillStatus.Posted;
                AuditHelper.ApplyUpdate(b2, _tenant);
                await _bills.UpdateAsync(b2, ct);
                await _bills.SaveChangesAsync(ct);

                return BaseResponse<PurchaseBillResponseDto>.Ok(await MapWithItemsAsync(b2, ct), "Posted.");
            }, cancellationToken, IsolationLevel.Serializable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Purchase bill post failed tenant {TenantId}", _tenant.TenantId);
            return BaseResponse<PurchaseBillResponseDto>.Fail("Post failed.");
        }
    }

    private bool IsGrnInFacility(PhrGoodsReceipt grn) =>
        !RequiresFacilityId || _tenant.FacilityId is null || grn.FacilityId == _tenant.FacilityId;

    private bool IsPoInFacilityScope(PhrPurchaseOrder po) =>
        !RequiresFacilityId || _tenant.FacilityId is null || po.FacilityId == _tenant.FacilityId;

    private async Task<bool> InvoiceExistsAsync(long supplierId, string invoiceNo, long? excludeBillId, CancellationToken ct)
    {
        var n = invoiceNo.Trim();
        var list = await _bills.ListAsync(
            b => !b.IsDeleted && b.SupplierId == supplierId && b.InvoiceNo == n,
            ct);
        if (excludeBillId is { } x)
            return list.Any(b => b.Id != x);
        return list.Count > 0;
    }

    private static string? ValidateLineInputsAgainstGrn(
        IReadOnlyList<PhrGoodsReceiptItem> grnLines,
        List<PurchaseBillLineInputDto> inputs)
    {
        var expected = grnLines.Select(l => l.Id).OrderBy(x => x).ToList();
        var got = inputs.Select(i => i.GoodsReceiptItemId).OrderBy(x => x).ToList();
        if (expected.Count != got.Count || !expected.SequenceEqual(got))
            return "Bill lines must include exactly the goods receipt items (no manual add/remove).";
        foreach (var inp in inputs)
        {
            if (inp.Quantity <= 0) return "Each line quantity must be greater than zero.";
            if (inp.Rate < 0) return "Rate cannot be negative.";
        }

        return null;
    }

    private async Task<string?> ValidateQuantitiesAgainstGrn(
        IReadOnlyList<PhrGoodsReceiptItem> grnLines,
        List<PurchaseBillLineInputDto> inputs,
        long? excludePurchaseBillId,
        CancellationToken ct)
    {
        var byId = grnLines.ToDictionary(i => i.Id, i => i);
        foreach (var inp in inputs)
        {
            if (!byId.TryGetValue(inp.GoodsReceiptItemId, out var grnLine))
                return "Invalid goods receipt item.";
            var postedElsewhere = await SumPostedQtyForGrnItemAsync(inp.GoodsReceiptItemId, excludePurchaseBillId, ct);
            if (postedElsewhere + inp.Quantity > grnLine.QuantityReceived + 0.0000001m)
                return $"Billed quantity cannot exceed GRN received quantity for line {grnLine.LineNum}.";
        }

        return null;
    }

    private async Task<decimal> SumPostedQtyForGrnItemAsync(long goodsReceiptItemId, long? excludePurchaseBillId, CancellationToken ct)
    {
        var postedBills = await _bills.ListAsync(
            b => !b.IsDeleted && b.Status == PharmacyPurchaseBillStatus.Posted,
            ct);
        var ids = postedBills
            .Where(b => excludePurchaseBillId is null || b.Id != excludePurchaseBillId)
            .Select(b => b.Id)
            .ToHashSet();
        if (ids.Count == 0) return 0m;
        var lines = await _items.ListAsync(
            i => !i.IsDeleted && ids.Contains(i.PurchaseBillId) && i.GoodsReceiptItemId == goodsReceiptItemId,
            ct);
        return lines.Sum(i => i.Quantity);
    }

    private static decimal PreviewSubtotal(IReadOnlyList<PhrGoodsReceiptItem> grnLines, List<PurchaseBillLineInputDto> inputs)
    {
        var byId = grnLines.ToDictionary(i => i.Id, i => i);
        decimal sub = 0m;
        foreach (var inp in inputs.OrderBy(i => byId[i.GoodsReceiptItemId].LineNum))
            sub += Math.Round(inp.Quantity * inp.Rate, 4, MidpointRounding.AwayFromZero);
        return sub;
    }

    private static string? ValidateFinancials(decimal subtotal, decimal discountAmount)
    {
        if (discountAmount < 0) return "Discount cannot be negative.";
        if (discountAmount > subtotal) return "Discount cannot exceed subtotal.";
        return null;
    }

    private static void ApplyTotals(
        PhrPurchaseBill bill,
        List<PurchaseBillLineInputDto> inputs,
        IReadOnlyList<PhrGoodsReceiptItem> grnLines,
        decimal discountAmount,
        decimal gstPercent,
        decimal otherTaxAmount)
    {
        var sub = PreviewSubtotal(grnLines, inputs);
        bill.SubTotal = sub;
        bill.DiscountAmount = discountAmount;
        bill.GstPercent = gstPercent;
        bill.OtherTaxAmount = otherTaxAmount;

        var taxable = Math.Max(0m, sub - discountAmount);
        bill.GstAmount = Math.Round(taxable * (gstPercent / 100m), 4, MidpointRounding.AwayFromZero);
        bill.NetAmount = Math.Round(sub - discountAmount + bill.GstAmount + otherTaxAmount, 4, MidpointRounding.AwayFromZero);
    }

    private async Task AddBillLinesAsync(
        long purchaseBillId,
        long goodsReceiptId,
        IReadOnlyList<PhrGoodsReceiptItem> grnLines,
        List<PurchaseBillLineInputDto> inputs,
        CancellationToken ct)
    {
        var byId = grnLines.ToDictionary(i => i.Id, i => i);
        var ordered = inputs.OrderBy(i => byId[i.GoodsReceiptItemId].LineNum).ToList();
        var lineNum = 1;
        foreach (var inp in ordered)
        {
            var grnLine = byId[inp.GoodsReceiptItemId];
            var amount = Math.Round(inp.Quantity * inp.Rate, 4, MidpointRounding.AwayFromZero);
            var line = new PhrPurchaseBillItem
            {
                PurchaseBillId = purchaseBillId,
                GoodsReceiptId = goodsReceiptId,
                GoodsReceiptItemId = inp.GoodsReceiptItemId,
                LineNum = lineNum++,
                Quantity = inp.Quantity,
                Rate = inp.Rate,
                Amount = amount,
                IsActive = true
            };
            AuditHelper.ApplyCreate(line, _tenant);
            line.FacilityId = _tenant.FacilityId;
            await _items.AddAsync(line, ct);
        }

        await _items.SaveChangesAsync(ct);
    }

    private async Task<PurchaseBillResponseDto> MapWithItemsAsync(PhrPurchaseBill bill, CancellationToken ct)
    {
        var dto = _mapper.Map<PurchaseBillResponseDto>(bill);
        var lines = await _items.ListAsync(i => i.PurchaseBillId == bill.Id && !i.IsDeleted, ct);
        var grnLineById = (await _grnItems.ListAsync(
                i => i.GoodsReceiptId == bill.GoodsReceiptId && !i.IsDeleted,
                ct))
            .ToDictionary(i => i.Id, i => i);
        var medIds = grnLineById.Values.Select(v => v.MedicineId).Distinct().ToList();
        var meds = await _medicines.ListAsync(m => medIds.Contains(m.Id) && !m.IsDeleted, ct);
        var medById = meds.ToDictionary(m => m.Id, m => m);

        dto.Items = lines.OrderBy(l => l.LineNum).Select(l =>
        {
            var gl = grnLineById[l.GoodsReceiptItemId];
            var medName = medById.TryGetValue(gl.MedicineId, out var m) ? m.MedicineName : "?";
            return new PurchaseBillItemResponseDto
            {
                Id = l.Id,
                PurchaseBillId = l.PurchaseBillId,
                GoodsReceiptId = l.GoodsReceiptId,
                GoodsReceiptItemId = l.GoodsReceiptItemId,
                LineNum = l.LineNum,
                MedicineId = gl.MedicineId,
                MedicineName = medName,
                MedicineBatchId = gl.MedicineBatchId,
                BatchNo = gl.BatchNo,
                Quantity = l.Quantity,
                Rate = l.Rate,
                Amount = l.Amount
            };
        }).ToList();
        return dto;
    }

    private async Task<string> NextBillNoAsync(CancellationToken ct)
    {
        var prefix = $"PB-{DateTime.UtcNow:yyyyMMdd}-";
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
