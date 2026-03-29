namespace HMSService.Application.DTOs.Extended;

public sealed class UpdatePrescriptionDto
{
    public string PrescriptionNo { get; set; }
    public long VisitId { get; set; }
    public long PatientId { get; set; }
    public long DoctorId { get; set; }
    public DateTime PrescribedOn { get; set; }
    public long PrescriptionStatusReferenceValueId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? Indication { get; set; }
    public string? Notes { get; set; }
}