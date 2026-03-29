using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrPharmacySale : BaseEntity
{
    public string SalesNo { get; set; } = null!;
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public long? DoctorId { get; set; }
    public DateTime SalesDate { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal? PaymentTotal { get; set; }
    public string? Notes { get; set; }
}