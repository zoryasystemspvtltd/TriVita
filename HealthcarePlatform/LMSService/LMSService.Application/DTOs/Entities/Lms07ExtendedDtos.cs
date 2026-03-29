namespace LMSService.Application.DTOs.Entities;

#region IAM (remaining)
public sealed class IamRoleResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public string RoleCode { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
}

public sealed class CreateIamRoleDto
{
    public string RoleCode { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
}

public sealed class UpdateIamRoleDto
{
    public string RoleCode { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
}

public sealed class IamPermissionResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public string PermissionCode { get; set; } = null!;
    public string PermissionName { get; set; } = null!;
    public string? ModuleCode { get; set; }
    public string? Description { get; set; }
}

public sealed class CreateIamPermissionDto
{
    public string PermissionCode { get; set; } = null!;
    public string PermissionName { get; set; } = null!;
    public string? ModuleCode { get; set; }
    public string? Description { get; set; }
}

public sealed class UpdateIamPermissionDto
{
    public string PermissionCode { get; set; } = null!;
    public string PermissionName { get; set; } = null!;
    public string? ModuleCode { get; set; }
    public string? Description { get; set; }
}

public sealed class IamRolePermissionResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}

public sealed class CreateIamRolePermissionDto
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}

public sealed class UpdateIamRolePermissionDto
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}

public sealed class IamUserRoleAssignmentResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public long? BusinessUnitId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class CreateIamUserRoleAssignmentDto
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public long? BusinessUnitId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class UpdateIamUserRoleAssignmentDto
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public long? BusinessUnitId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class IamUserFacilityScopeResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public long UserId { get; set; }
    public long GrantFacilityId { get; set; }
}

public sealed class CreateIamUserFacilityScopeDto
{
    public long UserId { get; set; }
    public long GrantFacilityId { get; set; }
}

public sealed class UpdateIamUserFacilityScopeDto
{
    public long UserId { get; set; }
    public long GrantFacilityId { get; set; }
}

public sealed class IamUserMfaFactorResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public long UserId { get; set; }
    public long MfaTypeReferenceValueId { get; set; }
    public string? SecretPayload { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? LastUsedOn { get; set; }
}

public sealed class CreateIamUserMfaFactorDto
{
    public long UserId { get; set; }
    public long MfaTypeReferenceValueId { get; set; }
    public string? SecretPayload { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPrimary { get; set; }
}

public sealed class UpdateIamUserMfaFactorDto
{
    public long UserId { get; set; }
    public long MfaTypeReferenceValueId { get; set; }
    public string? SecretPayload { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? LastUsedOn { get; set; }
}

public sealed class IamPasswordResetTokenResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public long UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public DateTime? ConsumedOn { get; set; }
    public long? RequestChannelReferenceValueId { get; set; }
}

public sealed class CreateIamPasswordResetTokenDto
{
    public long UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public DateTime? ConsumedOn { get; set; }
    public long? RequestChannelReferenceValueId { get; set; }
}

public sealed class UpdateIamPasswordResetTokenDto
{
    public long UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public DateTime? ConsumedOn { get; set; }
    public long? RequestChannelReferenceValueId { get; set; }
}

public sealed class IamUserSessionActivityResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public long UserId { get; set; }
    public long ActivityTypeReferenceValueId { get; set; }
    public DateTime ActivityOn { get; set; }
    public string? SessionTokenHash { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}

public sealed class CreateIamUserSessionActivityDto
{
    public long UserId { get; set; }
    public long ActivityTypeReferenceValueId { get; set; }
    public DateTime ActivityOn { get; set; }
    public string? SessionTokenHash { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}

public sealed class UpdateIamUserSessionActivityDto
{
    public long UserId { get; set; }
    public long ActivityTypeReferenceValueId { get; set; }
    public DateTime ActivityOn { get; set; }
    public string? SessionTokenHash { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}
#endregion

#region Catalog / billing lines
public sealed class TestPackageLineResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long TestPackageId { get; set; }
    public long TestMasterId { get; set; }
    public int LineNum { get; set; }
    public bool IsOptionalInPackage { get; set; }
}

public sealed class CreateTestPackageLineDto
{
    public long TestPackageId { get; set; }
    public long TestMasterId { get; set; }
    public int LineNum { get; set; }
    public bool IsOptionalInPackage { get; set; }
}

public sealed class UpdateTestPackageLineDto
{
    public long TestPackageId { get; set; }
    public long TestMasterId { get; set; }
    public int LineNum { get; set; }
    public bool IsOptionalInPackage { get; set; }
}

public sealed class TestPriceResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long? TestMasterId { get; set; }
    public long? TestPackageId { get; set; }
    public long? DepartmentId { get; set; }
    public long? PriceTierReferenceValueId { get; set; }
    public decimal RateAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class CreateTestPriceDto
{
    public long? TestMasterId { get; set; }
    public long? TestPackageId { get; set; }
    public long? DepartmentId { get; set; }
    public long? PriceTierReferenceValueId { get; set; }
    public decimal RateAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class UpdateTestPriceDto
{
    public long? TestMasterId { get; set; }
    public long? TestPackageId { get; set; }
    public long? DepartmentId { get; set; }
    public long? PriceTierReferenceValueId { get; set; }
    public decimal RateAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class LabInvoiceLineResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public int LineNum { get; set; }
    public long LineTypeReferenceValueId { get; set; }
    public long? LabOrderItemId { get; set; }
    public long? TestMasterId { get; set; }
    public long? TestPackageId { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineSubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? LineTotal { get; set; }
}

public sealed class CreateLabInvoiceLineDto
{
    public long LabInvoiceHeaderId { get; set; }
    public int LineNum { get; set; }
    public long LineTypeReferenceValueId { get; set; }
    public long? LabOrderItemId { get; set; }
    public long? TestMasterId { get; set; }
    public long? TestPackageId { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineSubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? LineTotal { get; set; }
}

public sealed class UpdateLabInvoiceLineDto
{
    public long LabInvoiceHeaderId { get; set; }
    public int LineNum { get; set; }
    public long LineTypeReferenceValueId { get; set; }
    public long? LabOrderItemId { get; set; }
    public long? TestMasterId { get; set; }
    public long? TestPackageId { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineSubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? LineTotal { get; set; }
}

public sealed class LabPaymentTransactionResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionOn { get; set; }
    public long TransactionStatusReferenceValueId { get; set; }
    public long? PaymentModeId { get; set; }
    public long? GatewayProviderReferenceValueId { get; set; }
    public string? ExternalTransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class CreateLabPaymentTransactionDto
{
    public long LabInvoiceHeaderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionOn { get; set; }
    public long TransactionStatusReferenceValueId { get; set; }
    public long? PaymentModeId { get; set; }
    public long? GatewayProviderReferenceValueId { get; set; }
    public string? ExternalTransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateLabPaymentTransactionDto
{
    public long LabInvoiceHeaderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionOn { get; set; }
    public long TransactionStatusReferenceValueId { get; set; }
    public long? PaymentModeId { get; set; }
    public long? GatewayProviderReferenceValueId { get; set; }
    public string? ExternalTransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class PatientWalletAccountResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long PatientId { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal CurrentBalance { get; set; }
}

public sealed class CreatePatientWalletAccountDto
{
    public long PatientId { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal CurrentBalance { get; set; }
}

public sealed class UpdatePatientWalletAccountDto
{
    public long PatientId { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal CurrentBalance { get; set; }
}

public sealed class PatientWalletTransactionResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long PatientWalletAccountId { get; set; }
    public decimal AmountDelta { get; set; }
    public long WalletTxnTypeReferenceValueId { get; set; }
    public DateTime TransactionOn { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public long? LabPaymentTransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class CreatePatientWalletTransactionDto
{
    public long PatientWalletAccountId { get; set; }
    public decimal AmountDelta { get; set; }
    public long WalletTxnTypeReferenceValueId { get; set; }
    public DateTime TransactionOn { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public long? LabPaymentTransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdatePatientWalletTransactionDto
{
    public long PatientWalletAccountId { get; set; }
    public decimal AmountDelta { get; set; }
    public long WalletTxnTypeReferenceValueId { get; set; }
    public DateTime TransactionOn { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public long? LabPaymentTransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}
#endregion

#region Referral
public sealed class ReferralDoctorProfileResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public string ReferralCode { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public long? LinkedDoctorId { get; set; }
    public string? HospitalAffiliation { get; set; }
    public long? PrimaryContactId { get; set; }
    public long? PrimaryAddressId { get; set; }
    public long? ReferralTypeReferenceValueId { get; set; }
}

public sealed class CreateReferralDoctorProfileDto
{
    public string ReferralCode { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public long? LinkedDoctorId { get; set; }
    public string? HospitalAffiliation { get; set; }
    public long? PrimaryContactId { get; set; }
    public long? PrimaryAddressId { get; set; }
    public long? ReferralTypeReferenceValueId { get; set; }
}

public sealed class UpdateReferralDoctorProfileDto
{
    public string ReferralCode { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public long? LinkedDoctorId { get; set; }
    public string? HospitalAffiliation { get; set; }
    public long? PrimaryContactId { get; set; }
    public long? PrimaryAddressId { get; set; }
    public long? ReferralTypeReferenceValueId { get; set; }
}

public sealed class ReferralFeeRuleResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long ReferralDoctorProfileId { get; set; }
    public long FeeModeReferenceValueId { get; set; }
    public decimal FeeValue { get; set; }
    public long ApplyScopeReferenceValueId { get; set; }
    public long? TestMasterId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class CreateReferralFeeRuleDto
{
    public long ReferralDoctorProfileId { get; set; }
    public long FeeModeReferenceValueId { get; set; }
    public decimal FeeValue { get; set; }
    public long ApplyScopeReferenceValueId { get; set; }
    public long? TestMasterId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class UpdateReferralFeeRuleDto
{
    public long ReferralDoctorProfileId { get; set; }
    public long FeeModeReferenceValueId { get; set; }
    public decimal FeeValue { get; set; }
    public long ApplyScopeReferenceValueId { get; set; }
    public long? TestMasterId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class ReferralFeeLedgerResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long ReferralDoctorProfileId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public long? LabInvoiceLineId { get; set; }
    public long? LabOrderItemId { get; set; }
    public decimal FeeAmount { get; set; }
    public long LedgerStatusReferenceValueId { get; set; }
    public DateTime AccruedOn { get; set; }
}

public sealed class CreateReferralFeeLedgerDto
{
    public long ReferralDoctorProfileId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public long? LabInvoiceLineId { get; set; }
    public long? LabOrderItemId { get; set; }
    public decimal FeeAmount { get; set; }
    public long LedgerStatusReferenceValueId { get; set; }
    public DateTime AccruedOn { get; set; }
}

public sealed class UpdateReferralFeeLedgerDto
{
    public long ReferralDoctorProfileId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public long? LabInvoiceLineId { get; set; }
    public long? LabOrderItemId { get; set; }
    public decimal FeeAmount { get; set; }
    public long LedgerStatusReferenceValueId { get; set; }
    public DateTime AccruedOn { get; set; }
}

public sealed class ReferralSettlementResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public string SettlementNo { get; set; } = null!;
    public long ReferralDoctorProfileId { get; set; }
    public DateTime PeriodStartOn { get; set; }
    public DateTime PeriodEndOn { get; set; }
    public decimal TotalSettledAmount { get; set; }
    public long SettlementStatusReferenceValueId { get; set; }
    public DateTime? SettledOn { get; set; }
    public string? PaymentReferenceNo { get; set; }
}

public sealed class CreateReferralSettlementDto
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

public sealed class UpdateReferralSettlementDto
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

public sealed class ReferralSettlementLineResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long ReferralSettlementId { get; set; }
    public long ReferralFeeLedgerId { get; set; }
    public decimal AppliedAmount { get; set; }
}

public sealed class CreateReferralSettlementLineDto
{
    public long ReferralSettlementId { get; set; }
    public long ReferralFeeLedgerId { get; set; }
    public decimal AppliedAmount { get; set; }
}

public sealed class UpdateReferralSettlementLineDto
{
    public long ReferralSettlementId { get; set; }
    public long ReferralFeeLedgerId { get; set; }
    public decimal AppliedAmount { get; set; }
}
#endregion

#region B2B
public sealed class B2BPartnerResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public string PartnerCode { get; set; } = null!;
    public string PartnerName { get; set; } = null!;
    public long? PartnerCategoryReferenceValueId { get; set; }
    public long? PrimaryAddressId { get; set; }
    public long? PrimaryContactId { get; set; }
    public string? ContractReference { get; set; }
}

public sealed class CreateB2BPartnerDto
{
    public string PartnerCode { get; set; } = null!;
    public string PartnerName { get; set; } = null!;
    public long? PartnerCategoryReferenceValueId { get; set; }
    public long? PrimaryAddressId { get; set; }
    public long? PrimaryContactId { get; set; }
    public string? ContractReference { get; set; }
}

public sealed class UpdateB2BPartnerDto
{
    public string PartnerCode { get; set; } = null!;
    public string PartnerName { get; set; } = null!;
    public long? PartnerCategoryReferenceValueId { get; set; }
    public long? PrimaryAddressId { get; set; }
    public long? PrimaryContactId { get; set; }
    public string? ContractReference { get; set; }
}

public sealed class B2BPartnerTestRateResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long B2BPartnerId { get; set; }
    public long TestMasterId { get; set; }
    public decimal? RateAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? ContractDocumentRef { get; set; }
}

public sealed class CreateB2BPartnerTestRateDto
{
    public long B2BPartnerId { get; set; }
    public long TestMasterId { get; set; }
    public decimal? RateAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? ContractDocumentRef { get; set; }
}

public sealed class UpdateB2BPartnerTestRateDto
{
    public long B2BPartnerId { get; set; }
    public long TestMasterId { get; set; }
    public decimal? RateAmount { get; set; }
    public decimal? DiscountPercent { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? ContractDocumentRef { get; set; }
}

public sealed class B2BPartnerCreditProfileResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long B2BPartnerId { get; set; }
    public decimal CreditLimitAmount { get; set; }
    public string? CreditCurrencyCode { get; set; }
    public int? PaymentTermsDays { get; set; }
    public int? GracePeriodDays { get; set; }
    public decimal UtilizedAmount { get; set; }
}

public sealed class CreateB2BPartnerCreditProfileDto
{
    public long B2BPartnerId { get; set; }
    public decimal CreditLimitAmount { get; set; }
    public string? CreditCurrencyCode { get; set; }
    public int? PaymentTermsDays { get; set; }
    public int? GracePeriodDays { get; set; }
    public decimal UtilizedAmount { get; set; }
}

public sealed class UpdateB2BPartnerCreditProfileDto
{
    public long B2BPartnerId { get; set; }
    public decimal CreditLimitAmount { get; set; }
    public string? CreditCurrencyCode { get; set; }
    public int? PaymentTermsDays { get; set; }
    public int? GracePeriodDays { get; set; }
    public decimal UtilizedAmount { get; set; }
}

public sealed class B2BCreditLedgerResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long B2BPartnerCreditProfileId { get; set; }
    public long MovementTypeReferenceValueId { get; set; }
    public decimal AmountDelta { get; set; }
    public DateTime PostedOn { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class CreateB2BCreditLedgerDto
{
    public long B2BPartnerCreditProfileId { get; set; }
    public long MovementTypeReferenceValueId { get; set; }
    public decimal AmountDelta { get; set; }
    public DateTime PostedOn { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateB2BCreditLedgerDto
{
    public long B2BPartnerCreditProfileId { get; set; }
    public long MovementTypeReferenceValueId { get; set; }
    public decimal AmountDelta { get; set; }
    public DateTime PostedOn { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class B2BPartnerBillingStatementResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public string StatementNo { get; set; } = null!;
    public long B2BPartnerId { get; set; }
    public DateTime PeriodStartOn { get; set; }
    public DateTime PeriodEndOn { get; set; }
    public decimal TotalAmount { get; set; }
    public long StatementStatusReferenceValueId { get; set; }
    public DateTime? IssuedOn { get; set; }
}

public sealed class CreateB2BPartnerBillingStatementDto
{
    public string StatementNo { get; set; } = null!;
    public long B2BPartnerId { get; set; }
    public DateTime PeriodStartOn { get; set; }
    public DateTime PeriodEndOn { get; set; }
    public decimal TotalAmount { get; set; }
    public long StatementStatusReferenceValueId { get; set; }
    public DateTime? IssuedOn { get; set; }
}

public sealed class UpdateB2BPartnerBillingStatementDto
{
    public string StatementNo { get; set; } = null!;
    public long B2BPartnerId { get; set; }
    public DateTime PeriodStartOn { get; set; }
    public DateTime PeriodEndOn { get; set; }
    public decimal TotalAmount { get; set; }
    public long StatementStatusReferenceValueId { get; set; }
    public DateTime? IssuedOn { get; set; }
}

public sealed class B2BPartnerBillingStatementLineResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long PartnerBillingStatementId { get; set; }
    public int LineNum { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public string? Description { get; set; }
    public decimal LineAmount { get; set; }
}

public sealed class CreateB2BPartnerBillingStatementLineDto
{
    public long PartnerBillingStatementId { get; set; }
    public int LineNum { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public string? Description { get; set; }
    public decimal LineAmount { get; set; }
}

public sealed class UpdateB2BPartnerBillingStatementLineDto
{
    public long PartnerBillingStatementId { get; set; }
    public int LineNum { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public string? Description { get; set; }
    public decimal LineAmount { get; set; }
}
#endregion

#region Reagents / maps / gates / finance / analytics / audit
public sealed class ReagentMasterResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public string ReagentCode { get; set; } = null!;
    public string ReagentName { get; set; } = null!;
    public long? DefaultUnitId { get; set; }
    public string? StorageNotes { get; set; }
}

public sealed class CreateReagentMasterDto
{
    public string ReagentCode { get; set; } = null!;
    public string ReagentName { get; set; } = null!;
    public long? DefaultUnitId { get; set; }
    public string? StorageNotes { get; set; }
}

public sealed class UpdateReagentMasterDto
{
    public string ReagentCode { get; set; } = null!;
    public string ReagentName { get; set; } = null!;
    public long? DefaultUnitId { get; set; }
    public string? StorageNotes { get; set; }
}

public sealed class ReagentBatchResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long ReagentMasterId { get; set; }
    public string LotNo { get; set; } = null!;
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public long? LabInventoryId { get; set; }
    public decimal? OpeningQuantity { get; set; }
    public decimal? CurrentQuantity { get; set; }
}

public sealed class CreateReagentBatchDto
{
    public long ReagentMasterId { get; set; }
    public string LotNo { get; set; } = null!;
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public long? LabInventoryId { get; set; }
    public decimal? OpeningQuantity { get; set; }
    public decimal? CurrentQuantity { get; set; }
}

public sealed class UpdateReagentBatchDto
{
    public long ReagentMasterId { get; set; }
    public string LotNo { get; set; } = null!;
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public long? LabInventoryId { get; set; }
    public decimal? OpeningQuantity { get; set; }
    public decimal? CurrentQuantity { get; set; }
}

public sealed class TestReagentMapResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long TestMasterId { get; set; }
    public long ReagentMasterId { get; set; }
    public decimal QuantityPerTest { get; set; }
    public long? UnitId { get; set; }
}

public sealed class CreateTestReagentMapDto
{
    public long TestMasterId { get; set; }
    public long ReagentMasterId { get; set; }
    public decimal QuantityPerTest { get; set; }
    public long? UnitId { get; set; }
}

public sealed class UpdateTestReagentMapDto
{
    public long TestMasterId { get; set; }
    public long ReagentMasterId { get; set; }
    public decimal QuantityPerTest { get; set; }
    public long? UnitId { get; set; }
}

public sealed class ReportPaymentGateResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long ReportHeaderId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public decimal MinimumPaidPercent { get; set; }
    public bool IsReleased { get; set; }
    public DateTime? ReleasedOn { get; set; }
    public long? ReleaseReasonReferenceValueId { get; set; }
}

public sealed class CreateReportPaymentGateDto
{
    public long ReportHeaderId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public decimal MinimumPaidPercent { get; set; }
    public bool IsReleased { get; set; }
    public DateTime? ReleasedOn { get; set; }
    public long? ReleaseReasonReferenceValueId { get; set; }
}

public sealed class UpdateReportPaymentGateDto
{
    public long ReportHeaderId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public decimal MinimumPaidPercent { get; set; }
    public bool IsReleased { get; set; }
    public DateTime? ReleasedOn { get; set; }
    public long? ReleaseReasonReferenceValueId { get; set; }
}

public sealed class FinanceLedgerEntryResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public DateTime EntryDate { get; set; }
    public long AccountCategoryReferenceValueId { get; set; }
    public long SourceTypeReferenceValueId { get; set; }
    public long SourceId { get; set; }
    public decimal Amount { get; set; }
    public long? DebitCreditReferenceValueId { get; set; }
    public long? PatientId { get; set; }
    public long? LabOrderId { get; set; }
    public string? Notes { get; set; }
}

public sealed class CreateFinanceLedgerEntryDto
{
    public DateTime EntryDate { get; set; }
    public long AccountCategoryReferenceValueId { get; set; }
    public long SourceTypeReferenceValueId { get; set; }
    public long SourceId { get; set; }
    public decimal Amount { get; set; }
    public long? DebitCreditReferenceValueId { get; set; }
    public long? PatientId { get; set; }
    public long? LabOrderId { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateFinanceLedgerEntryDto
{
    public DateTime EntryDate { get; set; }
    public long AccountCategoryReferenceValueId { get; set; }
    public long SourceTypeReferenceValueId { get; set; }
    public long SourceId { get; set; }
    public decimal Amount { get; set; }
    public long? DebitCreditReferenceValueId { get; set; }
    public long? PatientId { get; set; }
    public long? LabOrderId { get; set; }
    public string? Notes { get; set; }
}

public sealed class AnalyticsDailyFacilityRollupResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public DateTime StatDate { get; set; }
    public int LabOrderCount { get; set; }
    public int ReportIssuedCount { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal ReferralFeeAccrued { get; set; }
    public int? AvgTatMinutes { get; set; }
}

public sealed class CreateAnalyticsDailyFacilityRollupDto
{
    public DateTime StatDate { get; set; }
    public int LabOrderCount { get; set; }
    public int ReportIssuedCount { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal ReferralFeeAccrued { get; set; }
    public int? AvgTatMinutes { get; set; }
}

public sealed class UpdateAnalyticsDailyFacilityRollupDto
{
    public DateTime StatDate { get; set; }
    public int LabOrderCount { get; set; }
    public int ReportIssuedCount { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal ReferralFeeAccrued { get; set; }
    public int? AvgTatMinutes { get; set; }
}

public sealed class SecDataChangeAuditLogResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public long? UserId { get; set; }
    public long ActionTypeReferenceValueId { get; set; }
    public string EntitySchema { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public string? EntityKeyJson { get; set; }
    public string? ChangeSummary { get; set; }
    public string? CorrelationId { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
}

public sealed class CreateSecDataChangeAuditLogDto
{
    public long? UserId { get; set; }
    public long ActionTypeReferenceValueId { get; set; }
    public string EntitySchema { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public string? EntityKeyJson { get; set; }
    public string? ChangeSummary { get; set; }
    public string? CorrelationId { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
}

public sealed class UpdateSecDataChangeAuditLogDto
{
    public long? UserId { get; set; }
    public long ActionTypeReferenceValueId { get; set; }
    public string EntitySchema { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public string? EntityKeyJson { get; set; }
    public string? ChangeSummary { get; set; }
    public string? CorrelationId { get; set; }
    public string? ClientIp { get; set; }
    public string? UserAgent { get; set; }
}
#endregion
