namespace LMSService.Application.DTOs.Entities;

public sealed class ProcessingStageResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string StageCode { get; set; }
    public string StageName { get; set; }
    public int SequenceNo { get; set; }
    public string? StageNotes { get; set; }
}