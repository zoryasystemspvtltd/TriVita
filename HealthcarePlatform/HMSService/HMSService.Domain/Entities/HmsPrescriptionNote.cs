using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsPrescriptionNote : BaseEntity
{
    public long PrescriptionId { get; set; }
    public long NoteTypeReferenceValueId { get; set; }
    public string NoteText { get; set; } = null!;
}