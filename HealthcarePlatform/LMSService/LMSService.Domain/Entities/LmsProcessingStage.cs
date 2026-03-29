using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsProcessingStage : BaseEntity
{
    public string StageCode { get; set; } = null!;
    public string StageName { get; set; } = null!;
    public int SequenceNo { get; set; }
    public string? StageNotes { get; set; }
}