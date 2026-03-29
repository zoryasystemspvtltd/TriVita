namespace HMSService.Application.DTOs.Extended;

public sealed class PrescriptionNoteResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long PrescriptionId { get; set; }
    public long NoteTypeReferenceValueId { get; set; }
    public string NoteText { get; set; }
}