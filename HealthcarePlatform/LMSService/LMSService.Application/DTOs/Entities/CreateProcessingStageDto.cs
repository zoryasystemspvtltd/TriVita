namespace LMSService.Application.DTOs.Entities;

public sealed class CreateProcessingStageDto
{
    public string StageCode { get; set; }
    public string StageName { get; set; }
    public int SequenceNo { get; set; }
    public string? StageNotes { get; set; }
}