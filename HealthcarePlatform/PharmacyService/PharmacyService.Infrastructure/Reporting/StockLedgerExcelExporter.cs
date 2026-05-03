using ClosedXML.Excel;
using PharmacyService.Application.DTOs.Entities;

namespace PharmacyService.Infrastructure.Reporting;

internal static class StockLedgerExcelExporter
{
    public static byte[] BuildDetailed(
        string title,
        IReadOnlyList<string> filterLines,
        IReadOnlyList<StockLedgerDetailedRowDto> rows)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Detailed");
        var r = 1;
        ws.Cell(r, 1).Value = title;
        ws.Cell(r, 1).Style.Font.Bold = true;
        ws.Cell(r, 1).Style.Font.FontSize = 14;
        r++;
        foreach (var line in filterLines)
        {
            ws.Cell(r, 1).Value = line;
            r++;
        }

        r++;
        var hdr = new[]
        {
            "Transaction date", "Type", "Reference", "Medicine", "Batch", "Expiry",
            "Qty in", "Qty out", "Balance"
        };
        for (var c = 0; c < hdr.Length; c++)
        {
            ws.Cell(r, c + 1).Value = hdr[c];
            ws.Cell(r, c + 1).Style.Font.Bold = true;
            ws.Cell(r, c + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        r++;
        foreach (var row in rows)
        {
            ws.Cell(r, 1).Value = row.TransactionDate;
            ws.Cell(r, 1).Style.DateFormat.Format = "yyyy-mm-dd hh:mm";
            ws.Cell(r, 2).Value = row.TransactionType;
            ws.Cell(r, 3).Value = row.ReferenceNo;
            ws.Cell(r, 4).Value = row.MedicineName;
            ws.Cell(r, 5).Value = row.BatchNumber;
            ws.Cell(r, 6).Value = row.ExpiryDate;
            if (row.ExpiryDate is { } ex)
                ws.Cell(r, 6).Style.DateFormat.Format = "yyyy-mm-dd";
            ws.Cell(r, 7).Value = row.QuantityIn;
            ws.Cell(r, 8).Value = row.QuantityOut;
            ws.Cell(r, 9).Value = row.Balance;
            ws.Cell(r, 7).Style.NumberFormat.Format = "#,##0.####";
            ws.Cell(r, 8).Style.NumberFormat.Format = "#,##0.####";
            ws.Cell(r, 9).Style.NumberFormat.Format = "#,##0.####";
            r++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public static byte[] BuildSummary(
        string title,
        IReadOnlyList<string> filterLines,
        IReadOnlyList<StockLedgerSummaryReportRowDto> rows)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Summary");
        var r = 1;
        ws.Cell(r, 1).Value = title;
        ws.Cell(r, 1).Style.Font.Bold = true;
        ws.Cell(r, 1).Style.Font.FontSize = 14;
        r++;
        foreach (var line in filterLines)
        {
            ws.Cell(r, 1).Value = line;
            r++;
        }

        r++;
        var hdr = new[]
        {
            "Medicine", "Batch", "Expiry", "Opening qty", "Total in", "Total out", "Closing qty"
        };
        for (var c = 0; c < hdr.Length; c++)
        {
            ws.Cell(r, c + 1).Value = hdr[c];
            ws.Cell(r, c + 1).Style.Font.Bold = true;
            ws.Cell(r, c + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        r++;
        foreach (var row in rows)
        {
            ws.Cell(r, 1).Value = row.MedicineName;
            ws.Cell(r, 2).Value = row.BatchNumber;
            ws.Cell(r, 3).Value = row.ExpiryDate;
            if (row.ExpiryDate is { })
                ws.Cell(r, 3).Style.DateFormat.Format = "yyyy-mm-dd";
            ws.Cell(r, 4).Value = row.OpeningQty;
            ws.Cell(r, 5).Value = row.TotalIn;
            ws.Cell(r, 6).Value = row.TotalOut;
            ws.Cell(r, 7).Value = row.ClosingQty;
            for (var c = 4; c <= 7; c++)
                ws.Cell(r, c).Style.NumberFormat.Format = "#,##0.####";
            r++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
