using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsPatientMaster : BaseEntity
{
    public string Upid { get; set; } = null!;

    public long? SharedPatientId { get; set; }

    public string FullName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public long? GenderReferenceValueId { get; set; }

    public string? PrimaryPhone { get; set; }

    public string? PrimaryEmail { get; set; }
}

public sealed class HmsPatientFacilityLink : BaseEntity
{
    public long PatientMasterId { get; set; }

    public DateTime LinkedOn { get; set; }

    public string? Notes { get; set; }
}
