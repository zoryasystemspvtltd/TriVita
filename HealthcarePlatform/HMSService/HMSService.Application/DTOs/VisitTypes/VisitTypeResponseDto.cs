namespace HMSService.Application.DTOs.VisitTypes;

public sealed class VisitTypeResponseDto
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long? FacilityId { get; set; }

    public string VisitTypeCode { get; set; } = null!;

    public string VisitTypeName { get; set; } = null!;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }
}
