using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class IamUserAccountConfiguration : IEntityTypeConfiguration<IamUserAccount>
{
    public void Configure(EntityTypeBuilder<IamUserAccount> builder)
    {
        builder.ToTable("IAM_UserAccount");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.LoginName).HasMaxLength(120);
        builder.Property(e => e.DisplayName).HasMaxLength(250);
        builder.Property(e => e.Email).HasMaxLength(300);
        builder.Property(e => e.Phone).HasMaxLength(50);
        builder.Property(e => e.PasswordHash).HasMaxLength(500);
    }
}

public sealed class IamRoleConfiguration : IEntityTypeConfiguration<IamRole>
{
    public void Configure(EntityTypeBuilder<IamRole> builder)
    {
        builder.ToTable("IAM_Role");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.RoleCode).HasMaxLength(80);
        builder.Property(e => e.RoleName).HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}

public sealed class IamPermissionConfiguration : IEntityTypeConfiguration<IamPermission>
{
    public void Configure(EntityTypeBuilder<IamPermission> builder)
    {
        builder.ToTable("IAM_Permission");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PermissionCode).HasMaxLength(120);
        builder.Property(e => e.PermissionName).HasMaxLength(250);
        builder.Property(e => e.ModuleCode).HasMaxLength(80);
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}

public sealed class IamRolePermissionConfiguration : IEntityTypeConfiguration<IamRolePermission>
{
    public void Configure(EntityTypeBuilder<IamRolePermission> builder)
    {
        builder.ToTable("IAM_RolePermission");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class IamUserRoleAssignmentConfiguration : IEntityTypeConfiguration<IamUserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<IamUserRoleAssignment> builder)
    {
        builder.ToTable("IAM_UserRoleAssignment");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class IamUserFacilityScopeConfiguration : IEntityTypeConfiguration<IamUserFacilityScope>
{
    public void Configure(EntityTypeBuilder<IamUserFacilityScope> builder)
    {
        builder.ToTable("IAM_UserFacilityScope");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class IamUserMfaFactorConfiguration : IEntityTypeConfiguration<IamUserMfaFactor>
{
    public void Configure(EntityTypeBuilder<IamUserMfaFactor> builder)
    {
        builder.ToTable("IAM_UserMfaFactor");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class IamPasswordResetTokenConfiguration : IEntityTypeConfiguration<IamPasswordResetToken>
{
    public void Configure(EntityTypeBuilder<IamPasswordResetToken> builder)
    {
        builder.ToTable("IAM_PasswordResetToken");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.TokenHash).HasMaxLength(256);
    }
}

public sealed class IamUserSessionActivityConfiguration : IEntityTypeConfiguration<IamUserSessionActivity>
{
    public void Configure(EntityTypeBuilder<IamUserSessionActivity> builder)
    {
        builder.ToTable("IAM_UserSessionActivity");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.SessionTokenHash).HasMaxLength(256);
        builder.Property(e => e.ClientIp).HasMaxLength(64);
        builder.Property(e => e.UserAgent).HasMaxLength(500);
        builder.Property(e => e.FailureReason).HasMaxLength(500);
    }
}
