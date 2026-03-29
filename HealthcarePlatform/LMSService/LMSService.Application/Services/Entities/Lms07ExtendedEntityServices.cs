using AutoMapper;
using FluentValidation;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Extended;
using LMSService.Domain.Entities;
using LMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMSService.Application.Services.Entities;

#region IAM
public interface IIamRoleService
{
    Task<BaseResponse<IamRoleResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamRoleResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamRoleResponseDto>> CreateAsync(CreateIamRoleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamRoleResponseDto>> UpdateAsync(long id, UpdateIamRoleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamRoleService
    : LmsCrudServiceBase<IamRole, CreateIamRoleDto, UpdateIamRoleDto, IamRoleResponseDto, IamRoleService>, IIamRoleService
{
    public IamRoleService(IRepository<IamRole> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateIamRoleDto>? cv, IValidator<UpdateIamRoleDto>? uv, IFacilityTenantValidator fv, ILogger<IamRoleService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<IamRoleResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface IIamPermissionService
{
    Task<BaseResponse<IamPermissionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamPermissionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamPermissionResponseDto>> CreateAsync(CreateIamPermissionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamPermissionResponseDto>> UpdateAsync(long id, UpdateIamPermissionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamPermissionService
    : LmsCrudServiceBase<IamPermission, CreateIamPermissionDto, UpdateIamPermissionDto, IamPermissionResponseDto, IamPermissionService>, IIamPermissionService
{
    public IamPermissionService(IRepository<IamPermission> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateIamPermissionDto>? cv, IValidator<UpdateIamPermissionDto>? uv, IFacilityTenantValidator fv, ILogger<IamPermissionService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<IamPermissionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface IIamRolePermissionService
{
    Task<BaseResponse<IamRolePermissionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamRolePermissionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamRolePermissionResponseDto>> CreateAsync(CreateIamRolePermissionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamRolePermissionResponseDto>> UpdateAsync(long id, UpdateIamRolePermissionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamRolePermissionService
    : LmsCrudServiceBase<IamRolePermission, CreateIamRolePermissionDto, UpdateIamRolePermissionDto, IamRolePermissionResponseDto, IamRolePermissionService>, IIamRolePermissionService
{
    public IamRolePermissionService(IRepository<IamRolePermission> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateIamRolePermissionDto>? cv, IValidator<UpdateIamRolePermissionDto>? uv, IFacilityTenantValidator fv, ILogger<IamRolePermissionService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<IamRolePermissionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface IIamUserRoleAssignmentService
{
    Task<BaseResponse<IamUserRoleAssignmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamUserRoleAssignmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserRoleAssignmentResponseDto>> CreateAsync(CreateIamUserRoleAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserRoleAssignmentResponseDto>> UpdateAsync(long id, UpdateIamUserRoleAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamUserRoleAssignmentService
    : LmsCrudServiceBase<IamUserRoleAssignment, CreateIamUserRoleAssignmentDto, UpdateIamUserRoleAssignmentDto, IamUserRoleAssignmentResponseDto, IamUserRoleAssignmentService>, IIamUserRoleAssignmentService
{
    public IamUserRoleAssignmentService(IRepository<IamUserRoleAssignment> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateIamUserRoleAssignmentDto>? cv, IValidator<UpdateIamUserRoleAssignmentDto>? uv, IFacilityTenantValidator fv, ILogger<IamUserRoleAssignmentService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<IamUserRoleAssignmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface IIamUserFacilityScopeService
{
    Task<BaseResponse<IamUserFacilityScopeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamUserFacilityScopeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserFacilityScopeResponseDto>> CreateAsync(CreateIamUserFacilityScopeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserFacilityScopeResponseDto>> UpdateAsync(long id, UpdateIamUserFacilityScopeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamUserFacilityScopeService
    : LmsCrudServiceBase<IamUserFacilityScope, CreateIamUserFacilityScopeDto, UpdateIamUserFacilityScopeDto, IamUserFacilityScopeResponseDto, IamUserFacilityScopeService>, IIamUserFacilityScopeService
{
    public IamUserFacilityScopeService(IRepository<IamUserFacilityScope> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateIamUserFacilityScopeDto>? cv, IValidator<UpdateIamUserFacilityScopeDto>? uv, IFacilityTenantValidator fv, ILogger<IamUserFacilityScopeService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<IamUserFacilityScopeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface IIamUserMfaFactorService
{
    Task<BaseResponse<IamUserMfaFactorResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamUserMfaFactorResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserMfaFactorResponseDto>> CreateAsync(CreateIamUserMfaFactorDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserMfaFactorResponseDto>> UpdateAsync(long id, UpdateIamUserMfaFactorDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamUserMfaFactorService
    : LmsCrudServiceBase<IamUserMfaFactor, CreateIamUserMfaFactorDto, UpdateIamUserMfaFactorDto, IamUserMfaFactorResponseDto, IamUserMfaFactorService>, IIamUserMfaFactorService
{
    public IamUserMfaFactorService(IRepository<IamUserMfaFactor> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateIamUserMfaFactorDto>? cv, IValidator<UpdateIamUserMfaFactorDto>? uv, IFacilityTenantValidator fv, ILogger<IamUserMfaFactorService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<IamUserMfaFactorResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface IIamPasswordResetTokenService
{
    Task<BaseResponse<IamPasswordResetTokenResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamPasswordResetTokenResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamPasswordResetTokenResponseDto>> CreateAsync(CreateIamPasswordResetTokenDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamPasswordResetTokenResponseDto>> UpdateAsync(long id, UpdateIamPasswordResetTokenDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamPasswordResetTokenService
    : LmsCrudServiceBase<IamPasswordResetToken, CreateIamPasswordResetTokenDto, UpdateIamPasswordResetTokenDto, IamPasswordResetTokenResponseDto, IamPasswordResetTokenService>, IIamPasswordResetTokenService
{
    public IamPasswordResetTokenService(IRepository<IamPasswordResetToken> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateIamPasswordResetTokenDto>? cv, IValidator<UpdateIamPasswordResetTokenDto>? uv, IFacilityTenantValidator fv, ILogger<IamPasswordResetTokenService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<IamPasswordResetTokenResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface IIamUserSessionActivityService
{
    Task<BaseResponse<IamUserSessionActivityResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamUserSessionActivityResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserSessionActivityResponseDto>> CreateAsync(CreateIamUserSessionActivityDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserSessionActivityResponseDto>> UpdateAsync(long id, UpdateIamUserSessionActivityDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamUserSessionActivityService
    : LmsCrudServiceBase<IamUserSessionActivity, CreateIamUserSessionActivityDto, UpdateIamUserSessionActivityDto, IamUserSessionActivityResponseDto, IamUserSessionActivityService>, IIamUserSessionActivityService
{
    public IamUserSessionActivityService(IRepository<IamUserSessionActivity> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateIamUserSessionActivityDto>? cv, IValidator<UpdateIamUserSessionActivityDto>? uv, IFacilityTenantValidator fv, ILogger<IamUserSessionActivityService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<IamUserSessionActivityResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}
#endregion

#region Catalog / billing lines (facility-scoped)
public interface ILmsTestPackageLineService
{
    Task<BaseResponse<TestPackageLineResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestPackageLineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestPackageLineResponseDto>> CreateAsync(CreateTestPackageLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestPackageLineResponseDto>> UpdateAsync(long id, UpdateTestPackageLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsTestPackageLineService
    : LmsCrudServiceBase<LmsTestPackageLine, CreateTestPackageLineDto, UpdateTestPackageLineDto, TestPackageLineResponseDto, LmsTestPackageLineService>, ILmsTestPackageLineService
{
    public LmsTestPackageLineService(IRepository<LmsTestPackageLine> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateTestPackageLineDto>? cv, IValidator<UpdateTestPackageLineDto>? uv, IFacilityTenantValidator fv, ILogger<LmsTestPackageLineService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<TestPackageLineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsTestPriceService
{
    Task<BaseResponse<TestPriceResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestPriceResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestPriceResponseDto>> CreateAsync(CreateTestPriceDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestPriceResponseDto>> UpdateAsync(long id, UpdateTestPriceDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsTestPriceService
    : LmsCrudServiceBase<LmsTestPrice, CreateTestPriceDto, UpdateTestPriceDto, TestPriceResponseDto, LmsTestPriceService>, ILmsTestPriceService
{
    public LmsTestPriceService(IRepository<LmsTestPrice> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateTestPriceDto>? cv, IValidator<UpdateTestPriceDto>? uv, IFacilityTenantValidator fv, ILogger<LmsTestPriceService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<TestPriceResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsLabInvoiceLineService
{
    Task<BaseResponse<LabInvoiceLineResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabInvoiceLineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabInvoiceLineResponseDto>> CreateAsync(CreateLabInvoiceLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabInvoiceLineResponseDto>> UpdateAsync(long id, UpdateLabInvoiceLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsLabInvoiceLineService
    : LmsCrudServiceBase<LmsLabInvoiceLine, CreateLabInvoiceLineDto, UpdateLabInvoiceLineDto, LabInvoiceLineResponseDto, LmsLabInvoiceLineService>, ILmsLabInvoiceLineService
{
    public LmsLabInvoiceLineService(IRepository<LmsLabInvoiceLine> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateLabInvoiceLineDto>? cv, IValidator<UpdateLabInvoiceLineDto>? uv, IFacilityTenantValidator fv, ILogger<LmsLabInvoiceLineService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<LabInvoiceLineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsLabPaymentTransactionService
{
    Task<BaseResponse<LabPaymentTransactionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabPaymentTransactionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabPaymentTransactionResponseDto>> CreateAsync(CreateLabPaymentTransactionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabPaymentTransactionResponseDto>> UpdateAsync(long id, UpdateLabPaymentTransactionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsLabPaymentTransactionService
    : LmsCrudServiceBase<LmsLabPaymentTransaction, CreateLabPaymentTransactionDto, UpdateLabPaymentTransactionDto, LabPaymentTransactionResponseDto, LmsLabPaymentTransactionService>, ILmsLabPaymentTransactionService
{
    public LmsLabPaymentTransactionService(IRepository<LmsLabPaymentTransaction> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateLabPaymentTransactionDto>? cv, IValidator<UpdateLabPaymentTransactionDto>? uv, IFacilityTenantValidator fv, ILogger<LmsLabPaymentTransactionService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<LabPaymentTransactionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsPatientWalletAccountService
{
    Task<BaseResponse<PatientWalletAccountResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PatientWalletAccountResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PatientWalletAccountResponseDto>> CreateAsync(CreatePatientWalletAccountDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PatientWalletAccountResponseDto>> UpdateAsync(long id, UpdatePatientWalletAccountDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsPatientWalletAccountService
    : LmsCrudServiceBase<LmsPatientWalletAccount, CreatePatientWalletAccountDto, UpdatePatientWalletAccountDto, PatientWalletAccountResponseDto, LmsPatientWalletAccountService>, ILmsPatientWalletAccountService
{
    public LmsPatientWalletAccountService(IRepository<LmsPatientWalletAccount> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreatePatientWalletAccountDto>? cv, IValidator<UpdatePatientWalletAccountDto>? uv, IFacilityTenantValidator fv, ILogger<LmsPatientWalletAccountService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<PatientWalletAccountResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsPatientWalletTransactionService
{
    Task<BaseResponse<PatientWalletTransactionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PatientWalletTransactionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PatientWalletTransactionResponseDto>> CreateAsync(CreatePatientWalletTransactionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PatientWalletTransactionResponseDto>> UpdateAsync(long id, UpdatePatientWalletTransactionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsPatientWalletTransactionService
    : LmsCrudServiceBase<LmsPatientWalletTransaction, CreatePatientWalletTransactionDto, UpdatePatientWalletTransactionDto, PatientWalletTransactionResponseDto, LmsPatientWalletTransactionService>, ILmsPatientWalletTransactionService
{
    public LmsPatientWalletTransactionService(IRepository<LmsPatientWalletTransaction> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreatePatientWalletTransactionDto>? cv, IValidator<UpdatePatientWalletTransactionDto>? uv, IFacilityTenantValidator fv, ILogger<LmsPatientWalletTransactionService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<PatientWalletTransactionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}
#endregion

#region Referral / B2B
public interface ILmsReferralDoctorProfileService
{
    Task<BaseResponse<ReferralDoctorProfileResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReferralDoctorProfileResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralDoctorProfileResponseDto>> CreateAsync(CreateReferralDoctorProfileDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralDoctorProfileResponseDto>> UpdateAsync(long id, UpdateReferralDoctorProfileDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReferralDoctorProfileService
    : LmsCrudServiceBase<LmsReferralDoctorProfile, CreateReferralDoctorProfileDto, UpdateReferralDoctorProfileDto, ReferralDoctorProfileResponseDto, LmsReferralDoctorProfileService>, ILmsReferralDoctorProfileService
{
    public LmsReferralDoctorProfileService(IRepository<LmsReferralDoctorProfile> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateReferralDoctorProfileDto>? cv, IValidator<UpdateReferralDoctorProfileDto>? uv, IFacilityTenantValidator fv, ILogger<LmsReferralDoctorProfileService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<ReferralDoctorProfileResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsReferralFeeRuleService
{
    Task<BaseResponse<ReferralFeeRuleResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReferralFeeRuleResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralFeeRuleResponseDto>> CreateAsync(CreateReferralFeeRuleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralFeeRuleResponseDto>> UpdateAsync(long id, UpdateReferralFeeRuleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReferralFeeRuleService
    : LmsCrudServiceBase<LmsReferralFeeRule, CreateReferralFeeRuleDto, UpdateReferralFeeRuleDto, ReferralFeeRuleResponseDto, LmsReferralFeeRuleService>, ILmsReferralFeeRuleService
{
    public LmsReferralFeeRuleService(IRepository<LmsReferralFeeRule> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateReferralFeeRuleDto>? cv, IValidator<UpdateReferralFeeRuleDto>? uv, IFacilityTenantValidator fv, ILogger<LmsReferralFeeRuleService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<ReferralFeeRuleResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsReferralFeeLedgerService
{
    Task<BaseResponse<ReferralFeeLedgerResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReferralFeeLedgerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralFeeLedgerResponseDto>> CreateAsync(CreateReferralFeeLedgerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralFeeLedgerResponseDto>> UpdateAsync(long id, UpdateReferralFeeLedgerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReferralFeeLedgerService
    : LmsCrudServiceBase<LmsReferralFeeLedger, CreateReferralFeeLedgerDto, UpdateReferralFeeLedgerDto, ReferralFeeLedgerResponseDto, LmsReferralFeeLedgerService>, ILmsReferralFeeLedgerService
{
    public LmsReferralFeeLedgerService(IRepository<LmsReferralFeeLedger> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateReferralFeeLedgerDto>? cv, IValidator<UpdateReferralFeeLedgerDto>? uv, IFacilityTenantValidator fv, ILogger<LmsReferralFeeLedgerService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<ReferralFeeLedgerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsReferralSettlementService
{
    Task<BaseResponse<ReferralSettlementResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReferralSettlementResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralSettlementResponseDto>> CreateAsync(CreateReferralSettlementDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralSettlementResponseDto>> UpdateAsync(long id, UpdateReferralSettlementDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReferralSettlementService
    : LmsCrudServiceBase<LmsReferralSettlement, CreateReferralSettlementDto, UpdateReferralSettlementDto, ReferralSettlementResponseDto, LmsReferralSettlementService>, ILmsReferralSettlementService
{
    public LmsReferralSettlementService(IRepository<LmsReferralSettlement> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateReferralSettlementDto>? cv, IValidator<UpdateReferralSettlementDto>? uv, IFacilityTenantValidator fv, ILogger<LmsReferralSettlementService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<ReferralSettlementResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsReferralSettlementLineService
{
    Task<BaseResponse<ReferralSettlementLineResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReferralSettlementLineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralSettlementLineResponseDto>> CreateAsync(CreateReferralSettlementLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReferralSettlementLineResponseDto>> UpdateAsync(long id, UpdateReferralSettlementLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReferralSettlementLineService
    : LmsCrudServiceBase<LmsReferralSettlementLine, CreateReferralSettlementLineDto, UpdateReferralSettlementLineDto, ReferralSettlementLineResponseDto, LmsReferralSettlementLineService>, ILmsReferralSettlementLineService
{
    public LmsReferralSettlementLineService(IRepository<LmsReferralSettlementLine> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateReferralSettlementLineDto>? cv, IValidator<UpdateReferralSettlementLineDto>? uv, IFacilityTenantValidator fv, ILogger<LmsReferralSettlementLineService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<ReferralSettlementLineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsB2BPartnerService
{
    Task<BaseResponse<B2BPartnerResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<B2BPartnerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerResponseDto>> CreateAsync(CreateB2BPartnerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerResponseDto>> UpdateAsync(long id, UpdateB2BPartnerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsB2BPartnerService
    : LmsCrudServiceBase<LmsB2BPartner, CreateB2BPartnerDto, UpdateB2BPartnerDto, B2BPartnerResponseDto, LmsB2BPartnerService>, ILmsB2BPartnerService
{
    public LmsB2BPartnerService(IRepository<LmsB2BPartner> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateB2BPartnerDto>? cv, IValidator<UpdateB2BPartnerDto>? uv, IFacilityTenantValidator fv, ILogger<LmsB2BPartnerService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<B2BPartnerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsB2BPartnerTestRateService
{
    Task<BaseResponse<B2BPartnerTestRateResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<B2BPartnerTestRateResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerTestRateResponseDto>> CreateAsync(CreateB2BPartnerTestRateDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerTestRateResponseDto>> UpdateAsync(long id, UpdateB2BPartnerTestRateDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsB2BPartnerTestRateService
    : LmsCrudServiceBase<LmsB2BPartnerTestRate, CreateB2BPartnerTestRateDto, UpdateB2BPartnerTestRateDto, B2BPartnerTestRateResponseDto, LmsB2BPartnerTestRateService>, ILmsB2BPartnerTestRateService
{
    public LmsB2BPartnerTestRateService(IRepository<LmsB2BPartnerTestRate> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateB2BPartnerTestRateDto>? cv, IValidator<UpdateB2BPartnerTestRateDto>? uv, IFacilityTenantValidator fv, ILogger<LmsB2BPartnerTestRateService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<B2BPartnerTestRateResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsB2BPartnerCreditProfileService
{
    Task<BaseResponse<B2BPartnerCreditProfileResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<B2BPartnerCreditProfileResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerCreditProfileResponseDto>> CreateAsync(CreateB2BPartnerCreditProfileDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerCreditProfileResponseDto>> UpdateAsync(long id, UpdateB2BPartnerCreditProfileDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsB2BPartnerCreditProfileService
    : LmsCrudServiceBase<LmsB2BPartnerCreditProfile, CreateB2BPartnerCreditProfileDto, UpdateB2BPartnerCreditProfileDto, B2BPartnerCreditProfileResponseDto, LmsB2BPartnerCreditProfileService>, ILmsB2BPartnerCreditProfileService
{
    public LmsB2BPartnerCreditProfileService(IRepository<LmsB2BPartnerCreditProfile> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateB2BPartnerCreditProfileDto>? cv, IValidator<UpdateB2BPartnerCreditProfileDto>? uv, IFacilityTenantValidator fv, ILogger<LmsB2BPartnerCreditProfileService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<B2BPartnerCreditProfileResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsB2BCreditLedgerService
{
    Task<BaseResponse<B2BCreditLedgerResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<B2BCreditLedgerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BCreditLedgerResponseDto>> CreateAsync(CreateB2BCreditLedgerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BCreditLedgerResponseDto>> UpdateAsync(long id, UpdateB2BCreditLedgerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsB2BCreditLedgerService
    : LmsCrudServiceBase<LmsB2BCreditLedger, CreateB2BCreditLedgerDto, UpdateB2BCreditLedgerDto, B2BCreditLedgerResponseDto, LmsB2BCreditLedgerService>, ILmsB2BCreditLedgerService
{
    public LmsB2BCreditLedgerService(IRepository<LmsB2BCreditLedger> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateB2BCreditLedgerDto>? cv, IValidator<UpdateB2BCreditLedgerDto>? uv, IFacilityTenantValidator fv, ILogger<LmsB2BCreditLedgerService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<B2BCreditLedgerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsB2BPartnerBillingStatementService
{
    Task<BaseResponse<B2BPartnerBillingStatementResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<B2BPartnerBillingStatementResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerBillingStatementResponseDto>> CreateAsync(CreateB2BPartnerBillingStatementDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerBillingStatementResponseDto>> UpdateAsync(long id, UpdateB2BPartnerBillingStatementDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsB2BPartnerBillingStatementService
    : LmsCrudServiceBase<LmsB2BPartnerBillingStatement, CreateB2BPartnerBillingStatementDto, UpdateB2BPartnerBillingStatementDto, B2BPartnerBillingStatementResponseDto, LmsB2BPartnerBillingStatementService>, ILmsB2BPartnerBillingStatementService
{
    public LmsB2BPartnerBillingStatementService(IRepository<LmsB2BPartnerBillingStatement> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateB2BPartnerBillingStatementDto>? cv, IValidator<UpdateB2BPartnerBillingStatementDto>? uv, IFacilityTenantValidator fv, ILogger<LmsB2BPartnerBillingStatementService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<B2BPartnerBillingStatementResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsB2BPartnerBillingStatementLineService
{
    Task<BaseResponse<B2BPartnerBillingStatementLineResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<B2BPartnerBillingStatementLineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerBillingStatementLineResponseDto>> CreateAsync(CreateB2BPartnerBillingStatementLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<B2BPartnerBillingStatementLineResponseDto>> UpdateAsync(long id, UpdateB2BPartnerBillingStatementLineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsB2BPartnerBillingStatementLineService
    : LmsCrudServiceBase<LmsB2BPartnerBillingStatementLine, CreateB2BPartnerBillingStatementLineDto, UpdateB2BPartnerBillingStatementLineDto, B2BPartnerBillingStatementLineResponseDto, LmsB2BPartnerBillingStatementLineService>, ILmsB2BPartnerBillingStatementLineService
{
    public LmsB2BPartnerBillingStatementLineService(IRepository<LmsB2BPartnerBillingStatementLine> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateB2BPartnerBillingStatementLineDto>? cv, IValidator<UpdateB2BPartnerBillingStatementLineDto>? uv, IFacilityTenantValidator fv, ILogger<LmsB2BPartnerBillingStatementLineService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<B2BPartnerBillingStatementLineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}
#endregion

#region Reagents / finance / analytics / audit
public interface ILmsReagentMasterService
{
    Task<BaseResponse<ReagentMasterResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReagentMasterResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReagentMasterResponseDto>> CreateAsync(CreateReagentMasterDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReagentMasterResponseDto>> UpdateAsync(long id, UpdateReagentMasterDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReagentMasterService
    : LmsCrudServiceBase<LmsReagentMaster, CreateReagentMasterDto, UpdateReagentMasterDto, ReagentMasterResponseDto, LmsReagentMasterService>, ILmsReagentMasterService
{
    public LmsReagentMasterService(IRepository<LmsReagentMaster> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateReagentMasterDto>? cv, IValidator<UpdateReagentMasterDto>? uv, IFacilityTenantValidator fv, ILogger<LmsReagentMasterService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<ReagentMasterResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsReagentBatchService
{
    Task<BaseResponse<ReagentBatchResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReagentBatchResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReagentBatchResponseDto>> CreateAsync(CreateReagentBatchDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReagentBatchResponseDto>> UpdateAsync(long id, UpdateReagentBatchDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReagentBatchService
    : LmsCrudServiceBase<LmsReagentBatch, CreateReagentBatchDto, UpdateReagentBatchDto, ReagentBatchResponseDto, LmsReagentBatchService>, ILmsReagentBatchService
{
    public LmsReagentBatchService(IRepository<LmsReagentBatch> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateReagentBatchDto>? cv, IValidator<UpdateReagentBatchDto>? uv, IFacilityTenantValidator fv, ILogger<LmsReagentBatchService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<ReagentBatchResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsTestReagentMapService
{
    Task<BaseResponse<TestReagentMapResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestReagentMapResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestReagentMapResponseDto>> CreateAsync(CreateTestReagentMapDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestReagentMapResponseDto>> UpdateAsync(long id, UpdateTestReagentMapDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsTestReagentMapService
    : LmsCrudServiceBase<LmsTestReagentMap, CreateTestReagentMapDto, UpdateTestReagentMapDto, TestReagentMapResponseDto, LmsTestReagentMapService>, ILmsTestReagentMapService
{
    public LmsTestReagentMapService(IRepository<LmsTestReagentMap> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateTestReagentMapDto>? cv, IValidator<UpdateTestReagentMapDto>? uv, IFacilityTenantValidator fv, ILogger<LmsTestReagentMapService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<TestReagentMapResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsReportPaymentGateService
{
    Task<BaseResponse<ReportPaymentGateResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReportPaymentGateResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportPaymentGateResponseDto>> CreateAsync(CreateReportPaymentGateDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportPaymentGateResponseDto>> UpdateAsync(long id, UpdateReportPaymentGateDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReportPaymentGateService
    : LmsCrudServiceBase<LmsReportPaymentGate, CreateReportPaymentGateDto, UpdateReportPaymentGateDto, ReportPaymentGateResponseDto, LmsReportPaymentGateService>, ILmsReportPaymentGateService
{
    public LmsReportPaymentGateService(IRepository<LmsReportPaymentGate> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateReportPaymentGateDto>? cv, IValidator<UpdateReportPaymentGateDto>? uv, IFacilityTenantValidator fv, ILogger<LmsReportPaymentGateService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<ReportPaymentGateResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsFinanceLedgerEntryService
{
    Task<BaseResponse<FinanceLedgerEntryResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<FinanceLedgerEntryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<FinanceLedgerEntryResponseDto>> CreateAsync(CreateFinanceLedgerEntryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<FinanceLedgerEntryResponseDto>> UpdateAsync(long id, UpdateFinanceLedgerEntryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsFinanceLedgerEntryService
    : LmsCrudServiceBase<LmsFinanceLedgerEntry, CreateFinanceLedgerEntryDto, UpdateFinanceLedgerEntryDto, FinanceLedgerEntryResponseDto, LmsFinanceLedgerEntryService>, ILmsFinanceLedgerEntryService
{
    public LmsFinanceLedgerEntryService(IRepository<LmsFinanceLedgerEntry> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateFinanceLedgerEntryDto>? cv, IValidator<UpdateFinanceLedgerEntryDto>? uv, IFacilityTenantValidator fv, ILogger<LmsFinanceLedgerEntryService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<FinanceLedgerEntryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ILmsAnalyticsDailyFacilityRollupService
{
    Task<BaseResponse<AnalyticsDailyFacilityRollupResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<AnalyticsDailyFacilityRollupResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<AnalyticsDailyFacilityRollupResponseDto>> CreateAsync(CreateAnalyticsDailyFacilityRollupDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<AnalyticsDailyFacilityRollupResponseDto>> UpdateAsync(long id, UpdateAnalyticsDailyFacilityRollupDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsAnalyticsDailyFacilityRollupService
    : LmsCrudServiceBase<LmsAnalyticsDailyFacilityRollup, CreateAnalyticsDailyFacilityRollupDto, UpdateAnalyticsDailyFacilityRollupDto, AnalyticsDailyFacilityRollupResponseDto, LmsAnalyticsDailyFacilityRollupService>, ILmsAnalyticsDailyFacilityRollupService
{
    public LmsAnalyticsDailyFacilityRollupService(IRepository<LmsAnalyticsDailyFacilityRollup> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateAnalyticsDailyFacilityRollupDto>? cv, IValidator<UpdateAnalyticsDailyFacilityRollupDto>? uv, IFacilityTenantValidator fv, ILogger<LmsAnalyticsDailyFacilityRollupService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    public Task<BaseResponse<PagedResponse<AnalyticsDailyFacilityRollupResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}

public interface ISecDataChangeAuditLogService
{
    Task<BaseResponse<SecDataChangeAuditLogResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SecDataChangeAuditLogResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<SecDataChangeAuditLogResponseDto>> CreateAsync(CreateSecDataChangeAuditLogDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SecDataChangeAuditLogResponseDto>> UpdateAsync(long id, UpdateSecDataChangeAuditLogDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class SecDataChangeAuditLogService
    : LmsCrudServiceBase<SecDataChangeAuditLog, CreateSecDataChangeAuditLogDto, UpdateSecDataChangeAuditLogDto, SecDataChangeAuditLogResponseDto, SecDataChangeAuditLogService>, ISecDataChangeAuditLogService
{
    public SecDataChangeAuditLogService(IRepository<SecDataChangeAuditLog> repository, IMapper mapper, ITenantContext tenant,
        IValidator<CreateSecDataChangeAuditLogDto>? cv, IValidator<UpdateSecDataChangeAuditLogDto>? uv, IFacilityTenantValidator fv, ILogger<SecDataChangeAuditLogService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger) { }
    protected override bool RequiresFacilityId => false;
    public Task<BaseResponse<PagedResponse<SecDataChangeAuditLogResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
        => GetPagedCoreAsync(query, null, ct);
}
#endregion
