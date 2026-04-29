using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrForm : BaseEntity
{
    public string FormName { get; set; } = null!;
    public string FormCode { get; set; } = null!;
    public string? Description { get; set; }
}
