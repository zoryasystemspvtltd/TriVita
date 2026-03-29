namespace HMSService.Application.DTOs.VisitTypes;

public sealed class UpdateVisitTypeDto
{
    public string VisitTypeCode { get; set; } = null!;

    public string VisitTypeName { get; set; } = null!;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;
}
