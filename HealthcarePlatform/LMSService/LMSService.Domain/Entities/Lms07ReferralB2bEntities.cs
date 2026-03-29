using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsReferralDoctorProfile : BaseEntity
{
    public string ReferralCode { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public long? LinkedDoctorId { get; set; }
    public string? HospitalAffiliation { get; set; }
    public long? PrimaryContactId { get; set; }
    public long? PrimaryAddressId { get; set; }
    public long? ReferralTypeReferenceValueId { get; set; }
}

public sealed class LmsReferralFeeRule : BaseEntity
{
    public long ReferralDoctorProfileId { get; set; }
    public long FeeModeReferenceValueId { get; set; }
    public decimal FeeValue { get; set; }
    public long ApplyScopeReferenceValueId { get; set; }
    public long? TestMasterId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class LmsReferralFeeLedger : BaseEntity
{
    public long ReferralDoctorProfileId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public long? LabInvoiceLineId { get; set; }
    public long? LabOrderItemId { get; set; }
    public decimal FeeAmount { get; set; }
    public long LedgerStatusReferenceValueId { get; set; }
    public DateTime AccruedOn { get; set; }
}

public sealed class LmsReferralSettlement : BaseEntity
{
    public string SettlementNo { get; set; } = null!;
    public long ReferralDoctorProfileId { get; set; }
    public DateTime PeriodStartOn { get; set; }
    public DateTime PeriodEndOn { get; set; }
    public decimal TotalSettledAmount { get; set; }
    public long SettlementStatusReferenceValueId { get; set; }
    public DateTime? SettledOn { get; set; }
    public string? PaymentReferenceNo { get; set; }
}

public sealed class LmsReferralSettlementLine : BaseEntity
{
    public long ReferralSettlementId { get; set; }
    public long ReferralFeeLedgerId { get; set; }
    public decimal AppliedAmount { get; set; }
}

public sealed class LmsB2BPartner : BaseEntity
{
    public string PartnerCode { get; set; } = null!;
    public string PartnerName { get; set; } = null!;
    public long? PartnerCategoryReferenceValueId { get; set; }
    public long? PrimaryAddressId { get; set; }
    public long? PrimaryContactId { get; set; }
    public string? ContractReference { get; set; }
}

public sealed class LmsB2BPartnerTestRate : BaseEntity
{
    public long B2BPartnerId { get; set; }
    public long TestMasterId { get; set; }
    public decimal? RateAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? ContractDocumentRef { get; set; }
}

public sealed class LmsB2BPartnerCreditProfile : BaseEntity
{
    public long B2BPartnerId { get; set; }
    public decimal CreditLimitAmount { get; set; }
    public string? CreditCurrencyCode { get; set; }
    public int? PaymentTermsDays { get; set; }
    public int? GracePeriodDays { get; set; }
    public decimal UtilizedAmount { get; set; }
}

public sealed class LmsB2BCreditLedger : BaseEntity
{
    public long B2BPartnerCreditProfileId { get; set; }
    public long MovementTypeReferenceValueId { get; set; }
    public decimal AmountDelta { get; set; }
    public DateTime PostedOn { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class LmsB2BPartnerBillingStatement : BaseEntity
{
    public string StatementNo { get; set; } = null!;
    public long B2BPartnerId { get; set; }
    public DateTime PeriodStartOn { get; set; }
    public DateTime PeriodEndOn { get; set; }
    public decimal TotalAmount { get; set; }
    public long StatementStatusReferenceValueId { get; set; }
    public DateTime? IssuedOn { get; set; }
}

public sealed class LmsB2BPartnerBillingStatementLine : BaseEntity
{
    public long PartnerBillingStatementId { get; set; }
    public int LineNum { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public string? Description { get; set; }
    public decimal LineAmount { get; set; }
}
