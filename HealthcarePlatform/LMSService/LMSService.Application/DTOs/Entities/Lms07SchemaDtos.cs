namespace LMSService.Application.DTOs.Entities;

#region Lab invoice
public sealed class LabInvoiceHeaderResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public string InvoiceNo { get; set; } = null!;
    public long LabOrderId { get; set; }
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public long InvoiceStatusReferenceValueId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? TaxTotal { get; set; }
    public decimal? DiscountTotal { get; set; }
    public decimal? GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal? BalanceDue { get; set; }
    public string? CurrencyCode { get; set; }
}

public sealed class CreateLabInvoiceHeaderDto
{
    public string InvoiceNo { get; set; } = null!;
    public long LabOrderId { get; set; }
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public long InvoiceStatusReferenceValueId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? TaxTotal { get; set; }
    public decimal? DiscountTotal { get; set; }
    public decimal? GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal? BalanceDue { get; set; }
    public string? CurrencyCode { get; set; }
}

public sealed class UpdateLabInvoiceHeaderDto
{
    public string InvoiceNo { get; set; } = null!;
    public long LabOrderId { get; set; }
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public long InvoiceStatusReferenceValueId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? TaxTotal { get; set; }
    public decimal? DiscountTotal { get; set; }
    public decimal? GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal? BalanceDue { get; set; }
    public string? CurrencyCode { get; set; }
}
#endregion

#region Lab order context
public sealed class LabOrderContextResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long LabOrderId { get; set; }
    public long? B2BPartnerId { get; set; }
    public long? ReferralDoctorProfileId { get; set; }
    public long? SampleSourceReferenceValueId { get; set; }
    public long? BookingChannelReferenceValueId { get; set; }
    public DateTime? ExpectedReportOn { get; set; }
}

public sealed class CreateLabOrderContextDto
{
    public long LabOrderId { get; set; }
    public long? B2BPartnerId { get; set; }
    public long? ReferralDoctorProfileId { get; set; }
    public long? SampleSourceReferenceValueId { get; set; }
    public long? BookingChannelReferenceValueId { get; set; }
    public DateTime? ExpectedReportOn { get; set; }
}

public sealed class UpdateLabOrderContextDto
{
    public long LabOrderId { get; set; }
    public long? B2BPartnerId { get; set; }
    public long? ReferralDoctorProfileId { get; set; }
    public long? SampleSourceReferenceValueId { get; set; }
    public long? BookingChannelReferenceValueId { get; set; }
    public DateTime? ExpectedReportOn { get; set; }
}
#endregion

#region Test package
public sealed class TestPackageResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public string PackageCode { get; set; } = null!;
    public string PackageName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class CreateTestPackageDto
{
    public string PackageCode { get; set; } = null!;
    public string PackageName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class UpdateTestPackageDto
{
    public string PackageCode { get; set; } = null!;
    public string PackageName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}
#endregion

#region IAM user
public sealed class IamUserAccountResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public string LoginName { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public long? PatientId { get; set; }
    public long? DoctorId { get; set; }
    public long UserStatusReferenceValueId { get; set; }
    public DateTime? LastLoginOn { get; set; }
    public long? RegistrationSourceReferenceValueId { get; set; }
}

public sealed class CreateIamUserAccountDto
{
    public string LoginName { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PasswordHash { get; set; }
    public long? PatientId { get; set; }
    public long? DoctorId { get; set; }
    public long UserStatusReferenceValueId { get; set; }
    public long? RegistrationSourceReferenceValueId { get; set; }
}

public sealed class UpdateIamUserAccountDto
{
    public string LoginName { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PasswordHash { get; set; }
    public long? PatientId { get; set; }
    public long? DoctorId { get; set; }
    public long UserStatusReferenceValueId { get; set; }
    public long? RegistrationSourceReferenceValueId { get; set; }
    public DateTime? LastLoginOn { get; set; }
}
#endregion

#region Reagent consumption
public sealed class ReagentConsumptionLogResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long ReagentBatchId { get; set; }
    public decimal QuantityConsumed { get; set; }
    public DateTime ConsumedOn { get; set; }
    public long? LabOrderItemId { get; set; }
    public long? WorkQueueId { get; set; }
    public long? ConsumptionReasonReferenceValueId { get; set; }
    public string? Notes { get; set; }
}

public sealed class CreateReagentConsumptionLogDto
{
    public long ReagentBatchId { get; set; }
    public decimal QuantityConsumed { get; set; }
    public DateTime ConsumedOn { get; set; }
    public long? LabOrderItemId { get; set; }
    public long? WorkQueueId { get; set; }
    public long? ConsumptionReasonReferenceValueId { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateReagentConsumptionLogDto
{
    public long ReagentBatchId { get; set; }
    public decimal QuantityConsumed { get; set; }
    public DateTime ConsumedOn { get; set; }
    public long? LabOrderItemId { get; set; }
    public long? WorkQueueId { get; set; }
    public long? ConsumptionReasonReferenceValueId { get; set; }
    public string? Notes { get; set; }
}
#endregion
