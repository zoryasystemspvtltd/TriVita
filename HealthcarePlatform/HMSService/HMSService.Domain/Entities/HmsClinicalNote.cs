using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsClinicalNote : BaseEntity
{
    public long VisitId { get; set; }
    public long NoteTypeReferenceValueId { get; set; }
    public string? EncounterSection { get; set; }
    public string NoteText { get; set; } = null!;
    public string? StructuredPayload { get; set; }
    public long? AuthorDoctorId { get; set; }
}