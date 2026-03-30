using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsWard : BaseEntity
{
    public string WardCode { get; set; } = null!;

    public string WardName { get; set; } = null!;

    public long? WardCategoryReferenceValueId { get; set; }
}

public sealed class HmsBed : BaseEntity
{
    public long WardId { get; set; }

    public string BedCode { get; set; } = null!;

    public long? BedCategoryReferenceValueId { get; set; }

    public long BedOperationalStatusReferenceValueId { get; set; }

    public long? CurrentAdmissionId { get; set; }
}

public sealed class HmsAdmission : BaseEntity
{
    public string AdmissionNo { get; set; } = null!;

    public long PatientMasterId { get; set; }

    public long BedId { get; set; }

    public long AdmissionStatusReferenceValueId { get; set; }

    public DateTime AdmittedOn { get; set; }

    public DateTime? DischargedOn { get; set; }

    public long? AttendingDoctorId { get; set; }
}

public sealed class HmsAdmissionTransfer : BaseEntity
{
    public long AdmissionId { get; set; }

    public long FromBedId { get; set; }

    public long ToBedId { get; set; }

    public DateTime TransferredOn { get; set; }

    public string? Reason { get; set; }
}

public sealed class HmsHousekeepingStatus : BaseEntity
{
    public long BedId { get; set; }

    public long HousekeepingStatusReferenceValueId { get; set; }

    public DateTime RecordedOn { get; set; }

    public string? Notes { get; set; }
}
