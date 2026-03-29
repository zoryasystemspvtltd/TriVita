using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class IamUserAccount : BaseEntity
{
    public string LoginName { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PasswordHash { get; set; }
    public long? PatientId { get; set; }
    public long? DoctorId { get; set; }
    public long UserStatusReferenceValueId { get; set; }
    public DateTime? LastLoginOn { get; set; }
    public long? RegistrationSourceReferenceValueId { get; set; }
}

public sealed class IamRole : BaseEntity
{
    public string RoleCode { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
}

public sealed class IamPermission : BaseEntity
{
    public string PermissionCode { get; set; } = null!;
    public string PermissionName { get; set; } = null!;
    public string? ModuleCode { get; set; }
    public string? Description { get; set; }
}

public sealed class IamRolePermission : BaseEntity
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
}

public sealed class IamUserRoleAssignment : BaseEntity
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public long? BusinessUnitId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class IamUserFacilityScope : BaseEntity
{
    public long UserId { get; set; }
    public long GrantFacilityId { get; set; }
}

public sealed class IamUserMfaFactor : BaseEntity
{
    public long UserId { get; set; }
    public long MfaTypeReferenceValueId { get; set; }
    public string? SecretPayload { get; set; }
    public bool IsVerified { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? LastUsedOn { get; set; }
}

public sealed class IamPasswordResetToken : BaseEntity
{
    public long UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public DateTime? ConsumedOn { get; set; }
    public long? RequestChannelReferenceValueId { get; set; }
}

public sealed class IamUserSessionActivity : BaseEntity
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
