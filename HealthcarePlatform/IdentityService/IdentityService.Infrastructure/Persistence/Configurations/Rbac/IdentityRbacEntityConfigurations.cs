using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.Rbac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.Infrastructure.Persistence.Configurations.Rbac;

public sealed class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.ToTable("Identity_Role");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.RoleCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.RoleName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class IdentityPermissionConfiguration : IEntityTypeConfiguration<IdentityPermission>
{
    public void Configure(EntityTypeBuilder<IdentityPermission> builder)
    {
        builder.ToTable("Identity_Permission");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.PermissionCode).HasMaxLength(120).IsRequired();
        builder.Property(e => e.PermissionName).HasMaxLength(250).IsRequired();
        builder.Property(e => e.ModuleCode).HasMaxLength(80);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole> builder)
    {
        builder.ToTable("Identity_UserRole");
        builder.HasKey(e => new { e.UserId, e.RoleId });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<IdentityRole>()
            .WithMany()
            .HasForeignKey(e => new { e.TenantId, e.RoleId })
            .HasPrincipalKey(r => new { r.TenantId, r.Id });
    }
}

public sealed class IdentityRolePermissionConfiguration : IEntityTypeConfiguration<IdentityRolePermission>
{
    public void Configure(EntityTypeBuilder<IdentityRolePermission> builder)
    {
        builder.ToTable("Identity_RolePermission");
        builder.HasKey(e => new { e.RoleId, e.PermissionId });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasOne<IdentityRole>()
            .WithMany()
            .HasForeignKey(e => new { e.TenantId, e.RoleId })
            .HasPrincipalKey(r => new { r.TenantId, r.Id });
        builder.HasOne<IdentityPermission>()
            .WithMany()
            .HasForeignKey(e => new { e.TenantId, e.PermissionId })
            .HasPrincipalKey(p => new { p.TenantId, p.Id });
    }
}

public sealed class IdentityUserFacilityGrantConfiguration : IEntityTypeConfiguration<IdentityUserFacilityGrant>
{
    public void Configure(EntityTypeBuilder<IdentityUserFacilityGrant> builder)
    {
        builder.ToTable("Identity_UserFacilityGrant");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasIndex(e => new { e.UserId, e.GrantFacilityId }).IsUnique();
        builder.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class IdentityRefreshTokenConfiguration : IEntityTypeConfiguration<IdentityRefreshToken>
{
    public void Configure(EntityTypeBuilder<IdentityRefreshToken> builder)
    {
        builder.ToTable("Identity_RefreshToken");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TokenHash).HasMaxLength(256).IsRequired();
        builder.Property(e => e.ClientIp).HasMaxLength(64);
        builder.Property(e => e.UserAgent).HasMaxLength(512);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(e => e.TokenHash);
    }
}

public sealed class IdentityAccountLockoutStateConfiguration : IEntityTypeConfiguration<IdentityAccountLockoutState>
{
    public void Configure(EntityTypeBuilder<IdentityAccountLockoutState> builder)
    {
        builder.ToTable("Identity_AccountLockoutState");
        builder.HasKey(e => e.UserId);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class IdentityLoginAttemptConfiguration : IEntityTypeConfiguration<IdentityLoginAttempt>
{
    public void Configure(EntityTypeBuilder<IdentityLoginAttempt> builder)
    {
        builder.ToTable("Identity_LoginAttempt");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EmailAttempted).HasMaxLength(320).IsRequired();
        builder.Property(e => e.IpAddress).HasMaxLength(64);
        builder.Property(e => e.UserAgent).HasMaxLength(512);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public sealed class IdentityPasswordResetTokenConfiguration : IEntityTypeConfiguration<IdentityPasswordResetToken>
{
    public void Configure(EntityTypeBuilder<IdentityPasswordResetToken> builder)
    {
        builder.ToTable("Identity_PasswordResetToken");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TokenHash).HasMaxLength(256).IsRequired();
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class IdentityUserProfileConfiguration : IEntityTypeConfiguration<IdentityUserProfile>
{
    public void Configure(EntityTypeBuilder<IdentityUserProfile> builder)
    {
        builder.ToTable("Identity_UserProfile");
        builder.HasKey(e => e.UserId);
        builder.Property(e => e.DisplayName).HasMaxLength(250);
        builder.Property(e => e.Phone).HasMaxLength(50);
        builder.Property(e => e.TimeZoneId).HasMaxLength(100);
        builder.Property(e => e.AvatarUrl).HasMaxLength(500);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasOne<AppUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
