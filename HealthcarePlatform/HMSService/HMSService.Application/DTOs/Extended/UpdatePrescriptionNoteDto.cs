namespace HMSService.Application.DTOs.Extended;

public sealed class UpdatePrescriptionNoteDto
{
    public long PrescriptionId { get; set; }
    public long NoteTypeReferenceValueId { get; set; }
    public string NoteText { get; set; }
}