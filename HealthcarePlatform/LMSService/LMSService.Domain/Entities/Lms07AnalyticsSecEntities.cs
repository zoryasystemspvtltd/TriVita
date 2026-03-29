using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsFinanceLedgerEntry : BaseEntity
{
    public DateTime EntryDate { get; set; }
    public long AccountCategoryReferenceValueId { get; set; }
    public long SourceTypeReferenceValueId { get; set; }
    public long SourceId { get; set; }
    public decimal Amount { get; set; }
    public long? DebitCreditReferenceValueId { get; set; }
    public long? PatientId { get; set; }
    public long? LabOrderId { get; set; }
    public string? Notes { get; set; }
}

public sealed class LmsAnalyticsDailyFacilityRollup : BaseEntity
{
    public DateTime StatDate { get; set; }
    public int LabOrderCount { get; set; }
    public int ReportIssuedCount { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal ReferralFeeAccrued { get; set; }
    public int? AvgTatMinutes { get; set; }
}

public sealed class SecDataChangeAuditLog : BaseEntity
{
    public long? UserId { get; set; }
    public long ActionTypeReferenceValueId { get; set; }
    public string EntitySchema { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public string? EntityKeyJson { get; set; }
    public string? ChangeSummary { get; set; }
    public string? CorrelationId { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
}
