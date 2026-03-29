namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdateExpiryTrackingDto
{
    public long MedicineBatchId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int? DaysToExpiry { get; set; }
    public long? ExpiryAlertStatusReferenceValueId { get; set; }
    public DateTime? LastReviewedOn { get; set; }
    public long? ReviewedByDoctorId { get; set; }
    public string? Notes { get; set; }
}