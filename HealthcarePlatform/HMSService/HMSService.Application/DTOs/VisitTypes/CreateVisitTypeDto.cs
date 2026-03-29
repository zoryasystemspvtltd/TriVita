namespace HMSService.Application.DTOs.VisitTypes;

public sealed class CreateVisitTypeDto
{
    public string VisitTypeCode { get; set; } = null!;

    public string VisitTypeName { get; set; } = null!;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
