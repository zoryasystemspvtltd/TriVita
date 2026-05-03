using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;
using PharmacyService.Application.Services.Extended;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrStockLedgerService
{
    Task<BaseResponse<StockLedgerResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockLedgerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockLedgerResponseDto>>> GetReportPagedAsync(
        StockLedgerReportQuery query,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<IReadOnlyList<StockLedgerSummaryRowDto>>> GetSummaryReportAsync(
        StockLedgerReportQuery filter,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<StockLedgerResponseDto>> CreateAsync(CreateStockLedgerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockLedgerResponseDto>> UpdateAsync(long id, UpdateStockLedgerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockLedgerService : PhrCrudServiceBase<PhrStockLedger, CreateStockLedgerDto, UpdateStockLedgerDto, StockLedgerResponseDto, PhrStockLedgerService>, IPhrStockLedgerService
{
    public PhrStockLedgerService(
        IRepository<PhrStockLedger> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateStockLedgerDto>? createValidator,
        IValidator<UpdateStockLedgerDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrStockLedgerService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockLedgerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public async Task<BaseResponse<PagedResponse<StockLedgerResponseDto>>> GetReportPagedAsync(
        StockLedgerReportQuery query,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<PhrStockLedger, bool>>? extra = BuildReportFilter(query);
        return await GetPagedCoreAsync(query, extra, cancellationToken);
    }

    public async Task<BaseResponse<IReadOnlyList<StockLedgerSummaryRowDto>>> GetSummaryReportAsync(
        StockLedgerReportQuery filter,
        CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && Tenant.FacilityId is null)
            return BaseResponse<IReadOnlyList<StockLedgerSummaryRowDto>>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        Expression<Func<PhrStockLedger, bool>>? scoped = BuildReportFilter(filter);
        if (RequiresFacilityId && Tenant.FacilityId is { } facId)
        {
            Expression<Func<PhrStockLedger, bool>> fac = e => e.FacilityId == facId;
            scoped = scoped is null ? fac : AndAlsoExpressions(scoped, fac);
        }

        var list = await Repository.ListAsync(scoped, cancellationToken);
        var rows = list
            .GroupBy(x => new { x.TransactionType, x.MedicineId })
            .Select(g => new StockLedgerSummaryRowDto
            {
                TransactionType = g.Key.TransactionType,
                MedicineId = g.Key.MedicineId,
                NetQuantityDelta = g.Sum(x => x.QuantityDelta),
                EntryCount = g.Count()
            })
            .OrderBy(x => x.MedicineId)
            .ThenBy(x => x.TransactionType)
            .ToList();

        return BaseResponse<IReadOnlyList<StockLedgerSummaryRowDto>>.Ok(rows);
    }

    public override Task<BaseResponse<StockLedgerResponseDto>> CreateAsync(
        CreateStockLedgerDto dto,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(
            BaseResponse<StockLedgerResponseDto>.Fail(
                "Direct stock ledger entry is not allowed. Use GRN, stock adjustment, or sales flows."));

    public override Task<BaseResponse<StockLedgerResponseDto>> UpdateAsync(
        long id,
        UpdateStockLedgerDto dto,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(
            BaseResponse<StockLedgerResponseDto>.Fail("Stock ledger entries are immutable."));

    public override Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default) =>
        Task.FromResult(BaseResponse<object?>.Fail("Stock ledger entries cannot be deleted."));

    private Expression<Func<PhrStockLedger, bool>>? BuildReportFilter(StockLedgerReportQuery query)
    {
        Expression<Func<PhrStockLedger, bool>>? extra = null;

        if (query.TransactionType is { } tt)
        {
            Expression<Func<PhrStockLedger, bool>> p = e => e.TransactionType == tt;
            extra = extra is null ? p : AndAlsoExpressions(extra, p);
        }

        if (query.MedicineId is { } mid)
        {
            Expression<Func<PhrStockLedger, bool>> p = e => e.MedicineId == mid;
            extra = extra is null ? p : AndAlsoExpressions(extra, p);
        }

        if (query.MedicineBatchId is { } bid)
        {
            Expression<Func<PhrStockLedger, bool>> p = e => e.MedicineBatchId == bid;
            extra = extra is null ? p : AndAlsoExpressions(extra, p);
        }

        if (query.FromTransactionDate is { } from)
        {
            Expression<Func<PhrStockLedger, bool>> p = e => e.TransactionDate >= from;
            extra = extra is null ? p : AndAlsoExpressions(extra, p);
        }

        if (query.ToTransactionDate is { } to)
        {
            Expression<Func<PhrStockLedger, bool>> p = e => e.TransactionDate <= to;
            extra = extra is null ? p : AndAlsoExpressions(extra, p);
        }

        return extra;
    }

    private static Expression<Func<PhrStockLedger, bool>> AndAlsoExpressions(
        Expression<Func<PhrStockLedger, bool>> first,
        Expression<Func<PhrStockLedger, bool>> second)
    {
        var param = Expression.Parameter(typeof(PhrStockLedger), "e");
        var left = new PhrReplaceParamVisitor(first.Parameters[0], param).Visit(first.Body);
        var right = new PhrReplaceParamVisitor(second.Parameters[0], param).Visit(second.Body);
        return Expression.Lambda<Func<PhrStockLedger, bool>>(Expression.AndAlso(left!, right!), param);
    }

    private sealed class PhrReplaceParamVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public PhrReplaceParamVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node) =>
            node == _from ? _to : base.VisitParameter(node);
    }
}
