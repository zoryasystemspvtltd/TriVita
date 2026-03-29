namespace HMSService.Application.DTOs.Extended;

public sealed class UpdateClinicalNoteDto
{
    public long VisitId { get; set; }
    public long NoteTypeReferenceValueId { get; set; }
    public string? EncounterSection { get; set; }
    public string NoteText { get; set; }
    public string? StructuredPayload { get; set; }
    public long? AuthorDoctorId { get; set; }
}