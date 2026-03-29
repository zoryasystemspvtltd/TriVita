namespace HMSService.Contracts.Lis;

/// <summary>Inter-service DTO for HMS → LIS calls (no entity leakage).</summary>
public sealed class LisLabOrderSummaryDto
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long FacilityId { get; set; }

    public string LabOrderNo { get; set; } = null!;

    public long PatientId { get; set; }

    public long VisitId { get; set; }
}
