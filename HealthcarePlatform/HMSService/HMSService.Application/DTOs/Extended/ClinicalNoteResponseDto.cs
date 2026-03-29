namespace HMSService.Application.DTOs.Extended;

public sealed class ClinicalNoteResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long VisitId { get; set; }
    public long NoteTypeReferenceValueId { get; set; }
    public string? EncounterSection { get; set; }
    public string NoteText { get; set; }
    public string? StructuredPayload { get; set; }
    public long? AuthorDoctorId { get; set; }
}