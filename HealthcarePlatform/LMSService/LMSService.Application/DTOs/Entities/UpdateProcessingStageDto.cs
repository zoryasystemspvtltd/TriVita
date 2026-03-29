namespace LMSService.Application.DTOs.Entities;

public sealed class UpdateProcessingStageDto
{
    public string StageCode { get; set; }
    public string StageName { get; set; }
    public int SequenceNo { get; set; }
    public string? StageNotes { get; set; }
}