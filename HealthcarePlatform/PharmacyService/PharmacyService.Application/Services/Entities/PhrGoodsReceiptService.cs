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

public interface IPhrGoodsReceiptService
{
    Task<BaseResponse<GoodsReceiptResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<GoodsReceiptResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<GoodsReceiptResponseDto>> CreateAsync(CreateGoodsReceiptDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<GoodsReceiptResponseDto>> UpdateAsync(long id, UpdateGoodsReceiptDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrGoodsReceiptService : PhrCrudServiceBase<PhrGoodsReceipt, CreateGoodsReceiptDto, UpdateGoodsReceiptDto, GoodsReceiptResponseDto, PhrGoodsReceiptService>, IPhrGoodsReceiptService
{
    private readonly IRepository<PhrGoodsReceipt> _goodsReceipts;
    private readonly IRepository<PhrGoodsReceiptItem> _items;
    private readonly IRepository<PhrPurchaseOrder> _purchaseOrders;

    public PhrGoodsReceiptService(
        IRepository<PhrGoodsReceipt> repository,
        IRepository<PhrGoodsReceiptItem> items,
        IRepository<PhrPurchaseOrder> purchaseOrders,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateGoodsReceiptDto>? createValidator,
        IValidator<UpdateGoodsReceiptDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrGoodsReceiptService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _goodsReceipts = repository;
        _items = items;
        _purchaseOrders = purchaseOrders;
    }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<GoodsReceiptResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<GoodsReceiptResponseDto>> CreateAsync(
        CreateGoodsReceiptDto dto,
        CancellationToken cancellationToken = default)
    {
        var res = await base.CreateAsync(dto, cancellationToken);
        if (!res.Success || res.Data is null) return res;

        await RecalcTotalsAsync(res.Data.Id, cancellationToken);
        var gr = await _goodsReceipts.GetByIdAsync(res.Data.Id, cancellationToken);
        return gr is null
            ? BaseResponse<GoodsReceiptResponseDto>.Fail("GoodsReceipt not found.")
            : BaseResponse<GoodsReceiptResponseDto>.Ok(Mapper.Map<GoodsReceiptResponseDto>(gr), "Created.");
    }

    public override async Task<BaseResponse<GoodsReceiptResponseDto>> UpdateAsync(
        long id,
        UpdateGoodsReceiptDto dto,
        CancellationToken cancellationToken = default)
    {
        var res = await base.UpdateAsync(id, dto, cancellationToken);
        if (!res.Success) return res;

        await RecalcTotalsAsync(id, cancellationToken);
        var gr = await _goodsReceipts.GetByIdAsync(id, cancellationToken);
        return gr is null
            ? BaseResponse<GoodsReceiptResponseDto>.Fail("GoodsReceipt not found.")
            : BaseResponse<GoodsReceiptResponseDto>.Ok(Mapper.Map<GoodsReceiptResponseDto>(gr), "Updated.");
    }

    private async Task RecalcTotalsAsync(long goodsReceiptId, CancellationToken ct)
    {
        var gr = await _goodsReceipts.GetByIdAsync(goodsReceiptId, ct);
        if (gr is null) return;

        if (gr.PurchaseOrderId is { } poId)
        {
            var po = await _purchaseOrders.GetByIdAsync(poId, ct);
            if (po is not null)
            {
                gr.DiscountAmount = po.DiscountAmount;
                gr.GstPercent = po.GstPercent;
                gr.OtherTaxAmount = po.OtherTaxAmount;
            }
        }

        var lines = await _items.ListAsync(x => x.GoodsReceiptId == goodsReceiptId, ct);
        var subTotal = lines.Sum(x => x.LineTotal);

        gr.SubTotal = subTotal;
        var taxable = Math.Max(0m, gr.SubTotal - gr.DiscountAmount);
        gr.GstAmount = Math.Round(taxable * (gr.GstPercent / 100m), 4, MidpointRounding.AwayFromZero);
        gr.TotalAmount = Math.Round(gr.SubTotal - gr.DiscountAmount + gr.GstAmount + gr.OtherTaxAmount, 4, MidpointRounding.AwayFromZero);

        AuditHelper.ApplyUpdate(gr, Tenant);
        await _goodsReceipts.UpdateAsync(gr, ct);
        await _goodsReceipts.SaveChangesAsync(ct);
    }
}
