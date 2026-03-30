using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsOperationTheatre : BaseEntity
{
    public string TheatreCode { get; set; } = null!;
    public string TheatreName { get; set; } = null!;
    public long? DepartmentId { get; set; }
}

public sealed class HmsSurgerySchedule : BaseEntity
{
    public long OperationTheatreId { get; set; }
    public long PatientMasterId { get; set; }
    public long SurgeonDoctorId { get; set; }
    public DateTime ScheduledStartOn { get; set; }
    public DateTime? ScheduledEndOn { get; set; }
    public string? ProcedureSummary { get; set; }
    public long ScheduleStatusReferenceValueId { get; set; }
}

public sealed class HmsAnesthesiaRecord : BaseEntity
{
    public long SurgeryScheduleId { get; set; }
    public long? AnesthesiologistDoctorId { get; set; }
    public string? RecordJson { get; set; }
    public DateTime RecordedOn { get; set; }
}

public sealed class HmsPostOpRecord : BaseEntity
{
    public long SurgeryScheduleId { get; set; }
    public string? RecoveryNotes { get; set; }
    public DateTime RecordedOn { get; set; }
}

public sealed class HmsOtConsumable : BaseEntity
{
    public long SurgeryScheduleId { get; set; }
    public string ItemCode { get; set; } = null!;
    public string? ItemName { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public bool Billable { get; set; } = true;
}

public sealed class HmsPricingRule : BaseEntity
{
    public string RuleCode { get; set; } = null!;
    public string RuleName { get; set; } = null!;
    public long? TariffTypeReferenceValueId { get; set; }
    public string ServiceCode { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class HmsPackageDefinition : BaseEntity
{
    public string PackageCode { get; set; } = null!;
    public string PackageName { get; set; } = null!;
    public decimal BundlePrice { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class HmsPackageDefinitionLine : BaseEntity
{
    public long PackageDefinitionId { get; set; }
    public int LineNo { get; set; }
    public string ServiceCode { get; set; } = null!;
    public decimal Quantity { get; set; } = 1;
}

public sealed class HmsProformaInvoice : BaseEntity
{
    public string ProformaNo { get; set; } = null!;
    public long? PatientMasterId { get; set; }
    public long? VisitId { get; set; }
    public decimal GrandTotal { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? LinesJson { get; set; }
}

public sealed class HmsInsuranceProvider : BaseEntity
{
    public string ProviderCode { get; set; } = null!;
    public string ProviderName { get; set; } = null!;
    public long? TpaCategoryReferenceValueId { get; set; }
}

public sealed class HmsPreAuthorization : BaseEntity
{
    public string PreAuthNo { get; set; } = null!;
    public long InsuranceProviderId { get; set; }
    public long PatientMasterId { get; set; }
    public DateTime RequestedOn { get; set; }
    public long StatusReferenceValueId { get; set; }
    public decimal? ApprovedAmount { get; set; }
    public string? Notes { get; set; }
}

public sealed class HmsClaim : BaseEntity
{
    public string ClaimNo { get; set; } = null!;
    public long InsuranceProviderId { get; set; }
    public long PatientMasterId { get; set; }
    public long? BillingHeaderId { get; set; }
    public DateTime? SubmittedOn { get; set; }
    public long StatusReferenceValueId { get; set; }
    public decimal? ClaimAmount { get; set; }
}
