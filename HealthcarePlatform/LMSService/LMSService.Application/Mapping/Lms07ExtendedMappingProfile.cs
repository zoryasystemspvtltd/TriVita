using AutoMapper;
using LMSService.Application.DTOs.Entities;
using LMSService.Domain.Entities;

namespace LMSService.Application.Mapping;

/// <summary>AutoMapper maps for LMS schema 07 entities that were previously API-unexposed.</summary>
public sealed class Lms07ExtendedMappingProfile : Profile
{
    public Lms07ExtendedMappingProfile()
    {
        MapIam();
        MapCatalogBilling();
        MapReferralB2b();
        MapReagentsFinance();
    }

    private void MapIam()
    {
        CreateMap<IamRole, IamRoleResponseDto>();
        CreateMap<CreateIamRoleDto, IamRole>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateIamRoleDto, IamRole>().IgnoreBaseEntityOnUpdate();

        CreateMap<IamPermission, IamPermissionResponseDto>();
        CreateMap<CreateIamPermissionDto, IamPermission>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateIamPermissionDto, IamPermission>().IgnoreBaseEntityOnUpdate();

        CreateMap<IamRolePermission, IamRolePermissionResponseDto>();
        CreateMap<CreateIamRolePermissionDto, IamRolePermission>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateIamRolePermissionDto, IamRolePermission>().IgnoreBaseEntityOnUpdate();

        CreateMap<IamUserRoleAssignment, IamUserRoleAssignmentResponseDto>();
        CreateMap<CreateIamUserRoleAssignmentDto, IamUserRoleAssignment>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateIamUserRoleAssignmentDto, IamUserRoleAssignment>().IgnoreBaseEntityOnUpdate();

        CreateMap<IamUserFacilityScope, IamUserFacilityScopeResponseDto>();
        CreateMap<CreateIamUserFacilityScopeDto, IamUserFacilityScope>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateIamUserFacilityScopeDto, IamUserFacilityScope>().IgnoreBaseEntityOnUpdate();

        CreateMap<IamUserMfaFactor, IamUserMfaFactorResponseDto>();
        CreateMap<CreateIamUserMfaFactorDto, IamUserMfaFactor>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateIamUserMfaFactorDto, IamUserMfaFactor>().IgnoreBaseEntityOnUpdate();

        CreateMap<IamPasswordResetToken, IamPasswordResetTokenResponseDto>();
        CreateMap<CreateIamPasswordResetTokenDto, IamPasswordResetToken>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateIamPasswordResetTokenDto, IamPasswordResetToken>().IgnoreBaseEntityOnUpdate();

        CreateMap<IamUserSessionActivity, IamUserSessionActivityResponseDto>();
        CreateMap<CreateIamUserSessionActivityDto, IamUserSessionActivity>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateIamUserSessionActivityDto, IamUserSessionActivity>().IgnoreBaseEntityOnUpdate();
    }

    private void MapCatalogBilling()
    {
        CreateMap<LmsTestPackageLine, TestPackageLineResponseDto>();
        CreateMap<CreateTestPackageLineDto, LmsTestPackageLine>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateTestPackageLineDto, LmsTestPackageLine>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsTestPrice, TestPriceResponseDto>();
        CreateMap<CreateTestPriceDto, LmsTestPrice>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateTestPriceDto, LmsTestPrice>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsLabInvoiceLine, LabInvoiceLineResponseDto>();
        CreateMap<CreateLabInvoiceLineDto, LmsLabInvoiceLine>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateLabInvoiceLineDto, LmsLabInvoiceLine>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsLabPaymentTransaction, LabPaymentTransactionResponseDto>();
        CreateMap<CreateLabPaymentTransactionDto, LmsLabPaymentTransaction>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateLabPaymentTransactionDto, LmsLabPaymentTransaction>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsPatientWalletAccount, PatientWalletAccountResponseDto>();
        CreateMap<CreatePatientWalletAccountDto, LmsPatientWalletAccount>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdatePatientWalletAccountDto, LmsPatientWalletAccount>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsPatientWalletTransaction, PatientWalletTransactionResponseDto>();
        CreateMap<CreatePatientWalletTransactionDto, LmsPatientWalletTransaction>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdatePatientWalletTransactionDto, LmsPatientWalletTransaction>().IgnoreBaseEntityOnUpdate();
    }

    private void MapReferralB2b()
    {
        CreateMap<LmsReferralDoctorProfile, ReferralDoctorProfileResponseDto>();
        CreateMap<CreateReferralDoctorProfileDto, LmsReferralDoctorProfile>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateReferralDoctorProfileDto, LmsReferralDoctorProfile>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsReferralFeeRule, ReferralFeeRuleResponseDto>();
        CreateMap<CreateReferralFeeRuleDto, LmsReferralFeeRule>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateReferralFeeRuleDto, LmsReferralFeeRule>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsReferralFeeLedger, ReferralFeeLedgerResponseDto>();
        CreateMap<CreateReferralFeeLedgerDto, LmsReferralFeeLedger>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateReferralFeeLedgerDto, LmsReferralFeeLedger>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsReferralSettlement, ReferralSettlementResponseDto>();
        CreateMap<CreateReferralSettlementDto, LmsReferralSettlement>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateReferralSettlementDto, LmsReferralSettlement>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsReferralSettlementLine, ReferralSettlementLineResponseDto>();
        CreateMap<CreateReferralSettlementLineDto, LmsReferralSettlementLine>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateReferralSettlementLineDto, LmsReferralSettlementLine>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsB2BPartner, B2BPartnerResponseDto>();
        CreateMap<CreateB2BPartnerDto, LmsB2BPartner>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateB2BPartnerDto, LmsB2BPartner>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsB2BPartnerTestRate, B2BPartnerTestRateResponseDto>();
        CreateMap<CreateB2BPartnerTestRateDto, LmsB2BPartnerTestRate>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateB2BPartnerTestRateDto, LmsB2BPartnerTestRate>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsB2BPartnerCreditProfile, B2BPartnerCreditProfileResponseDto>();
        CreateMap<CreateB2BPartnerCreditProfileDto, LmsB2BPartnerCreditProfile>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateB2BPartnerCreditProfileDto, LmsB2BPartnerCreditProfile>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsB2BCreditLedger, B2BCreditLedgerResponseDto>();
        CreateMap<CreateB2BCreditLedgerDto, LmsB2BCreditLedger>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateB2BCreditLedgerDto, LmsB2BCreditLedger>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsB2BPartnerBillingStatement, B2BPartnerBillingStatementResponseDto>();
        CreateMap<CreateB2BPartnerBillingStatementDto, LmsB2BPartnerBillingStatement>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateB2BPartnerBillingStatementDto, LmsB2BPartnerBillingStatement>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsB2BPartnerBillingStatementLine, B2BPartnerBillingStatementLineResponseDto>();
        CreateMap<CreateB2BPartnerBillingStatementLineDto, LmsB2BPartnerBillingStatementLine>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateB2BPartnerBillingStatementLineDto, LmsB2BPartnerBillingStatementLine>().IgnoreBaseEntityOnUpdate();
    }

    private void MapReagentsFinance()
    {
        CreateMap<LmsReagentMaster, ReagentMasterResponseDto>();
        CreateMap<CreateReagentMasterDto, LmsReagentMaster>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateReagentMasterDto, LmsReagentMaster>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsReagentBatch, ReagentBatchResponseDto>();
        CreateMap<CreateReagentBatchDto, LmsReagentBatch>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateReagentBatchDto, LmsReagentBatch>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsTestReagentMap, TestReagentMapResponseDto>();
        CreateMap<CreateTestReagentMapDto, LmsTestReagentMap>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateTestReagentMapDto, LmsTestReagentMap>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsReportPaymentGate, ReportPaymentGateResponseDto>();
        CreateMap<CreateReportPaymentGateDto, LmsReportPaymentGate>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateReportPaymentGateDto, LmsReportPaymentGate>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsFinanceLedgerEntry, FinanceLedgerEntryResponseDto>();
        CreateMap<CreateFinanceLedgerEntryDto, LmsFinanceLedgerEntry>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateFinanceLedgerEntryDto, LmsFinanceLedgerEntry>().IgnoreBaseEntityOnUpdate();

        CreateMap<LmsAnalyticsDailyFacilityRollup, AnalyticsDailyFacilityRollupResponseDto>();
        CreateMap<CreateAnalyticsDailyFacilityRollupDto, LmsAnalyticsDailyFacilityRollup>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateAnalyticsDailyFacilityRollupDto, LmsAnalyticsDailyFacilityRollup>().IgnoreBaseEntityOnUpdate();

        CreateMap<SecDataChangeAuditLog, SecDataChangeAuditLogResponseDto>();
        CreateMap<CreateSecDataChangeAuditLogDto, SecDataChangeAuditLog>().IgnoreBaseEntityOnCreate();
        CreateMap<UpdateSecDataChangeAuditLogDto, SecDataChangeAuditLog>().IgnoreBaseEntityOnUpdate();
    }
}
