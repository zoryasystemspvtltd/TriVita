using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrExpiryTracking : BaseEntity
{
    public long MedicineBatchId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int? DaysToExpiry { get; set; }
    public long? ExpiryAlertStatusReferenceValueId { get; set; }
    public DateTime? LastReviewedOn { get; set; }
    public long? ReviewedByDoctorId { get; set; }
    public string? Notes { get; set; }
}