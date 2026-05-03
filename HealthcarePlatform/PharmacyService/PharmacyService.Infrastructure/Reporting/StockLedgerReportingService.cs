using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Enums;
using PharmacyService.Infrastructure.Persistence;

namespace PharmacyService.Infrastructure.Reporting;

public sealed class StockLedgerReportingService : IStockLedgerReportingService
{
    private const int MaxExportRows = 200_000;

    private readonly PharmacyDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly ILogger<StockLedgerReportingService> _logger;

    public StockLedgerReportingService(
        PharmacyDbContext db,
        ITenantContext tenant,
        ILogger<StockLedgerReportingService> logger)
    {
        _db = db;
        _tenant = tenant;
        _logger = logger;
    }

    public async Task<BaseResponse<PagedResponse<StockLedgerDetailedRowDto>>> GetDetailedAsync(
        StockLedgerReportQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetRange(query, out var rangeStart, out var rangeEndEx, out var rangeErr))
            return BaseResponse<PagedResponse<StockLedgerDetailedRowDto>>.Fail(rangeErr!);
        if (!TryResolveFacility(query, out var fac, out var facErr))
            return BaseResponse<PagedResponse<StockLedgerDetailedRowDto>>.Fail(facErr!);

        var search = query.Search?.Trim();
        var baseQ = BuildDetailedJoin(fac, rangeStart, rangeEndEx, query, search);
        var sorted = ApplyDetailedSort(baseQ, query.SortBy, query.SortDescending);

        var total = await sorted.CountAsync(cancellationToken);
        var slice = await sorted
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = slice.Select(MapDetailed).ToList();
        return BaseResponse<PagedResponse<StockLedgerDetailedRowDto>>.Ok(new PagedResponse<StockLedgerDetailedRowDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        });
    }

    public async Task<BaseResponse<PagedResponse<StockLedgerSummaryReportRowDto>>> GetSummaryAsync(
        StockLedgerReportQuery query,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetRange(query, out var rangeStart, out var rangeEndEx, out var rangeErr))
            return BaseResponse<PagedResponse<StockLedgerSummaryReportRowDto>>.Fail(rangeErr!);
        if (!TryResolveFacility(query, out var fac, out var facErr))
            return BaseResponse<PagedResponse<StockLedgerSummaryReportRowDto>>.Fail(facErr!);

        var joined = BuildSummaryQuery(fac, rangeStart, rangeEndEx, query);
        var sorted = ApplySummarySort(joined, query.SortBy, query.SortDescending);

        var total = await sorted.CountAsync(cancellationToken);
        var items = await sorted
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return BaseResponse<PagedResponse<StockLedgerSummaryReportRowDto>>.Ok(
            new PagedResponse<StockLedgerSummaryReportRowDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = total
            });
    }

    public async Task<BaseResponse<byte[]>> ExportDetailedExcelAsync(
        StockLedgerReportQuery filter,
        CancellationToken cancellationToken = default)
    {
        var q = CloneForExport(filter);
        if (!TryGetRange(q, out var rangeStart, out var rangeEndEx, out var rangeErr))
            return BaseResponse<byte[]>.Fail(rangeErr!);
        if (!TryResolveFacility(q, out var fac, out var facErr))
            return BaseResponse<byte[]>.Fail(facErr!);

        var search = q.Search?.Trim();
        var baseQ = BuildDetailedJoin(fac, rangeStart, rangeEndEx, q, search);
        var sorted = ApplyDetailedSort(baseQ, q.SortBy, q.SortDescending);

        var total = await sorted.CountAsync(cancellationToken);
        if (total > MaxExportRows)
            _logger.LogWarning("Stock ledger detailed export capped tenant {Tenant} total {Total}", _tenant.TenantId, total);

        var slice = await sorted.Take(MaxExportRows).ToListAsync(cancellationToken);
        var rows = slice.Select(MapDetailed).ToList();
        var filterLines = BuildFilterLinesForExport(q, rangeStart, rangeEndEx, fac, total > MaxExportRows ? MaxExportRows : null, total);
        var bytes = StockLedgerExcelExporter.BuildDetailed("Stock ledger — detailed", filterLines, rows);
        return BaseResponse<byte[]>.Ok(bytes);
    }

    public async Task<BaseResponse<byte[]>> ExportSummaryExcelAsync(
        StockLedgerReportQuery filter,
        CancellationToken cancellationToken = default)
    {
        var q = CloneForExport(filter);
        if (!TryGetRange(q, out var rangeStart, out var rangeEndEx, out var rangeErr))
            return BaseResponse<byte[]>.Fail(rangeErr!);
        if (!TryResolveFacility(q, out var fac, out var facErr))
            return BaseResponse<byte[]>.Fail(facErr!);

        var joined = BuildSummaryQuery(fac, rangeStart, rangeEndEx, q);
        var sorted = ApplySummarySort(joined, q.SortBy, q.SortDescending);
        var total = await sorted.CountAsync(cancellationToken);
        if (total > MaxExportRows)
            _logger.LogWarning("Stock ledger summary export capped tenant {Tenant}", _tenant.TenantId);

        var rows = await sorted.Take(MaxExportRows).ToListAsync(cancellationToken);
        var filterLines = BuildFilterLinesForExport(q, rangeStart, rangeEndEx, fac, total > MaxExportRows ? MaxExportRows : null, total);
        var bytes = StockLedgerExcelExporter.BuildSummary("Stock ledger — summary", filterLines, rows);
        return BaseResponse<byte[]>.Ok(bytes);
    }

    private IQueryable<StockLedgerDetailJoin> BuildDetailedJoin(
        long fac,
        DateTime rangeStart,
        DateTime rangeEndEx,
        StockLedgerReportQuery query,
        string? search) =>
        from sl in _db.PhrStockLedgers.AsNoTracking()
        join m in _db.PhrMedicines.AsNoTracking() on sl.MedicineId equals m.Id
        join b in _db.PhrMedicineBatches.AsNoTracking() on sl.MedicineBatchId equals b.Id
        where sl.FacilityId == fac
              && sl.TransactionDate >= rangeStart
              && sl.TransactionDate < rangeEndEx
              && (query.TransactionType == null || sl.TransactionType == query.TransactionType)
              && (query.MedicineId == null || sl.MedicineId == query.MedicineId)
              && (query.MedicineBatchId == null || sl.MedicineBatchId == query.MedicineBatchId)
              && (query.SupplierId == null || sl.GrnSupplierId == query.SupplierId)
              && (query.SalePartyId == null
                  || sl.SalePatientId == query.SalePartyId
                  || sl.SaleCustomerId == query.SalePartyId)
              && (string.IsNullOrEmpty(search) || m.MedicineName.Contains(search))
        select new StockLedgerDetailJoin(sl, m, b);

    private IQueryable<StockLedgerSummaryReportRowDto> BuildSummaryQuery(
        long fac,
        DateTime rangeStart,
        DateTime rangeEndEx,
        StockLedgerReportQuery query)
    {
        var search = query.Search?.Trim();

        var ledgerFiltered =
            from sl in _db.PhrStockLedgers.AsNoTracking()
            where sl.FacilityId == fac
                  && sl.TransactionDate < rangeEndEx
                  && (query.TransactionType == null || sl.TransactionType == query.TransactionType)
                  && (query.MedicineId == null || sl.MedicineId == query.MedicineId)
                  && (query.MedicineBatchId == null || sl.MedicineBatchId == query.MedicineBatchId)
                  && (query.SupplierId == null || sl.GrnSupplierId == query.SupplierId)
                  && (query.SalePartyId == null
                      || sl.SalePatientId == query.SalePartyId
                      || sl.SaleCustomerId == query.SalePartyId)
            select sl;

        if (!string.IsNullOrEmpty(search))
        {
            ledgerFiltered =
                from sl in ledgerFiltered
                join m in _db.PhrMedicines.AsNoTracking() on sl.MedicineId equals m.Id
                where m.MedicineName.Contains(search!)
                select sl;
        }

        var grouped =
            from sl in ledgerFiltered
            group sl by new { sl.MedicineId, sl.MedicineBatchId }
            into g
            let opening = g.Sum(x => x.TransactionDate < rangeStart ? x.QuantityDelta : 0m)
            let tin = g.Sum(x =>
                x.TransactionDate >= rangeStart && x.TransactionDate < rangeEndEx && x.QuantityDelta > 0 ? x.QuantityDelta : 0m)
            let tout = g.Sum(x =>
                x.TransactionDate >= rangeStart && x.TransactionDate < rangeEndEx && x.QuantityDelta < 0 ? -x.QuantityDelta : 0m)
            where opening != 0 || tin != 0 || tout != 0
            select new
            {
                g.Key.MedicineId,
                g.Key.MedicineBatchId,
                OpeningQty = opening,
                TotalIn = tin,
                TotalOut = tout,
                ClosingQty = opening + tin - tout
            };

        return from x in grouped
            join m in _db.PhrMedicines.AsNoTracking() on x.MedicineId equals m.Id
            join b in _db.PhrMedicineBatches.AsNoTracking() on x.MedicineBatchId equals b.Id
            select new StockLedgerSummaryReportRowDto
            {
                MedicineName = m.MedicineName,
                BatchNumber = b.BatchNo,
                ExpiryDate = b.ExpiryDate,
                OpeningQty = x.OpeningQty,
                TotalIn = x.TotalIn,
                TotalOut = x.TotalOut,
                ClosingQty = x.ClosingQty
            };
    }

    private static StockLedgerDetailedRowDto MapDetailed(StockLedgerDetailJoin x) =>
        new()
        {
            TransactionDate = x.Sl.TransactionDate,
            TransactionType = TxLabel(x.Sl.TransactionType),
            ReferenceNo = string.IsNullOrWhiteSpace(x.Sl.SourceReference) ? "—" : x.Sl.SourceReference!,
            MedicineName = x.M.MedicineName,
            BatchNumber = x.B.BatchNo,
            ExpiryDate = x.B.ExpiryDate,
            QuantityIn = x.Sl.QuantityDelta > 0 ? x.Sl.QuantityDelta : 0,
            QuantityOut = x.Sl.QuantityDelta < 0 ? -x.Sl.QuantityDelta : 0,
            Balance = x.Sl.AfterQty
        };

    private bool TryResolveFacility(StockLedgerReportQuery query, out long facilityId, out string? error)
    {
        if (query.FacilityId is { } qf)
        {
            if (_tenant.FacilityId is { } tf && tf != qf)
            {
                facilityId = 0;
                error = "Query facilityId must match the authenticated facility scope.";
                return false;
            }

            facilityId = qf;
            error = null;
            return true;
        }

        if (_tenant.FacilityId is { } tf2)
        {
            facilityId = tf2;
            error = null;
            return true;
        }

        facilityId = 0;
        error = "FacilityId is required (header X-Facility-Id, query facilityId, or claim facility_id).";
        return false;
    }

    private static bool TryGetRange(StockLedgerReportQuery query, out DateTime rangeStart, out DateTime rangeEndEx, out string? error)
    {
        if (query.FromTransactionDate is null || query.ToTransactionDate is null)
        {
            rangeStart = default;
            rangeEndEx = default;
            error = "FromTransactionDate and ToTransactionDate are required.";
            return false;
        }

        rangeStart = query.FromTransactionDate.Value.Date;
        rangeEndEx = query.ToTransactionDate.Value.Date.AddDays(1);
        if (rangeEndEx <= rangeStart)
        {
            error = "ToTransactionDate must be on or after FromTransactionDate.";
            rangeStart = default;
            rangeEndEx = default;
            return false;
        }

        error = null;
        return true;
    }

    private static StockLedgerReportQuery CloneForExport(StockLedgerReportQuery filter) =>
        new()
        {
            Page = 1,
            PageSize = MaxExportRows,
            SortBy = filter.SortBy,
            SortDescending = filter.SortDescending,
            Search = filter.Search,
            FromTransactionDate = filter.FromTransactionDate,
            ToTransactionDate = filter.ToTransactionDate,
            TransactionType = filter.TransactionType,
            MedicineId = filter.MedicineId,
            MedicineBatchId = filter.MedicineBatchId,
            SupplierId = filter.SupplierId,
            SalePartyId = filter.SalePartyId,
            FacilityId = filter.FacilityId
        };

    private List<string> BuildFilterLinesForExport(
        StockLedgerReportQuery q,
        DateTime rangeStart,
        DateTime rangeEndEx,
        long facilityId,
        int? rowCap,
        int totalMatching)
    {
        var toInclusive = rangeEndEx.AddDays(-1);
        var lines = new List<string>
        {
            $"Facility: {facilityId}",
            $"Date range: {rangeStart:yyyy-MM-dd} through {toInclusive:yyyy-MM-dd} (inclusive)"
        };
        if (q.TransactionType is { } tt)
            lines.Add($"Transaction type: {TxLabel(tt)}");
        if (q.MedicineId is { } mid)
            lines.Add($"Medicine filter (master id): {mid}");
        if (q.MedicineBatchId is { } bid)
            lines.Add($"Batch filter (master id): {bid}");
        if (q.SupplierId is { } sid)
            lines.Add($"Supplier filter (master id): {sid}");
        if (q.SalePartyId is { } pid)
            lines.Add($"Customer / patient filter (party id): {pid}");
        if (!string.IsNullOrWhiteSpace(q.Search))
            lines.Add($"Search: {q.Search}");
        if (rowCap is { } cap)
            lines.Add($"Export row cap applied: {cap} of {totalMatching} matching rows.");
        return lines;
    }

    private static string TxLabel(StockLedgerTransactionType t) =>
        t switch
        {
            StockLedgerTransactionType.GRN => "GRN",
            StockLedgerTransactionType.SALE => "SALE",
            StockLedgerTransactionType.ADJUSTMENT => "ADJUSTMENT",
            _ => t.ToString()
        };

    private static IQueryable<StockLedgerDetailJoin> ApplyDetailedSort(
        IQueryable<StockLedgerDetailJoin> q,
        string? sortBy,
        bool desc)
    {
        var key = (sortBy ?? "transactiondate").Trim().ToLowerInvariant();
        return key switch
        {
            "transactiontype" => desc
                ? q.OrderByDescending(x => x.Sl.TransactionType).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.Sl.TransactionType).ThenBy(x => x.Sl.Id),
            "referenceno" => desc
                ? q.OrderByDescending(x => x.Sl.SourceReference).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.Sl.SourceReference).ThenBy(x => x.Sl.Id),
            "medicinename" => desc
                ? q.OrderByDescending(x => x.M.MedicineName).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.M.MedicineName).ThenBy(x => x.Sl.Id),
            "batchnumber" => desc
                ? q.OrderByDescending(x => x.B.BatchNo).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.B.BatchNo).ThenBy(x => x.Sl.Id),
            "expirydate" => desc
                ? q.OrderByDescending(x => x.B.ExpiryDate).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.B.ExpiryDate).ThenBy(x => x.Sl.Id),
            "quantityin" => desc
                ? q.OrderByDescending(x => x.Sl.QuantityDelta > 0 ? x.Sl.QuantityDelta : 0m).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.Sl.QuantityDelta > 0 ? x.Sl.QuantityDelta : 0m).ThenBy(x => x.Sl.Id),
            "quantityout" => desc
                ? q.OrderByDescending(x => x.Sl.QuantityDelta < 0 ? -x.Sl.QuantityDelta : 0m).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.Sl.QuantityDelta < 0 ? -x.Sl.QuantityDelta : 0m).ThenBy(x => x.Sl.Id),
            "balance" => desc
                ? q.OrderByDescending(x => x.Sl.AfterQty).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.Sl.AfterQty).ThenBy(x => x.Sl.Id),
            _ => desc
                ? q.OrderByDescending(x => x.Sl.TransactionDate).ThenByDescending(x => x.Sl.Id)
                : q.OrderBy(x => x.Sl.TransactionDate).ThenBy(x => x.Sl.Id)
        };
    }

    private static IQueryable<StockLedgerSummaryReportRowDto> ApplySummarySort(
        IQueryable<StockLedgerSummaryReportRowDto> q,
        string? sortBy,
        bool desc)
    {
        var key = (sortBy ?? "medicinename").Trim().ToLowerInvariant();
        return key switch
        {
            "batchnumber" => desc
                ? q.OrderByDescending(x => x.BatchNumber).ThenByDescending(x => x.MedicineName)
                : q.OrderBy(x => x.BatchNumber).ThenBy(x => x.MedicineName),
            "expirydate" => desc
                ? q.OrderByDescending(x => x.ExpiryDate).ThenBy(x => x.MedicineName)
                : q.OrderBy(x => x.ExpiryDate).ThenBy(x => x.MedicineName),
            "openingqty" => desc
                ? q.OrderByDescending(x => x.OpeningQty).ThenBy(x => x.MedicineName)
                : q.OrderBy(x => x.OpeningQty).ThenBy(x => x.MedicineName),
            "totalin" => desc
                ? q.OrderByDescending(x => x.TotalIn).ThenBy(x => x.MedicineName)
                : q.OrderBy(x => x.TotalIn).ThenBy(x => x.MedicineName),
            "totalout" => desc
                ? q.OrderByDescending(x => x.TotalOut).ThenBy(x => x.MedicineName)
                : q.OrderBy(x => x.TotalOut).ThenBy(x => x.MedicineName),
            "closingqty" => desc
                ? q.OrderByDescending(x => x.ClosingQty).ThenBy(x => x.MedicineName)
                : q.OrderBy(x => x.ClosingQty).ThenBy(x => x.MedicineName),
            _ => desc
                ? q.OrderByDescending(x => x.MedicineName).ThenByDescending(x => x.BatchNumber)
                : q.OrderBy(x => x.MedicineName).ThenBy(x => x.BatchNumber)
        };
    }

    private sealed class StockLedgerDetailJoin
    {
        public StockLedgerDetailJoin(PhrStockLedger sl, PhrMedicine m, PhrMedicineBatch b)
        {
            Sl = sl;
            M = m;
            B = b;
        }

        public PhrStockLedger Sl { get; }
        public PhrMedicine M { get; }
        public PhrMedicineBatch B { get; }
    }
}
