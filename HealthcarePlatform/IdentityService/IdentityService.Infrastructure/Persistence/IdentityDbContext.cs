using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.Rbac;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

public sealed class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();

    public DbSet<IdentityRole> IdentityRoles => Set<IdentityRole>();

    public DbSet<IdentityPermission> IdentityPermissions => Set<IdentityPermission>();

    public DbSet<IdentityUserRole> IdentityUserRoles => Set<IdentityUserRole>();

    public DbSet<IdentityRolePermission> IdentityRolePermissions => Set<IdentityRolePermission>();

    public DbSet<IdentityUserFacilityGrant> IdentityUserFacilityGrants => Set<IdentityUserFacilityGrant>();

    public DbSet<IdentityRefreshToken> IdentityRefreshTokens => Set<IdentityRefreshToken>();

    public DbSet<IdentityAccountLockoutState> IdentityAccountLockoutStates => Set<IdentityAccountLockoutState>();

    public DbSet<IdentityLoginAttempt> IdentityLoginAttempts => Set<IdentityLoginAttempt>();

    public DbSet<IdentityPasswordResetToken> IdentityPasswordResetTokens => Set<IdentityPasswordResetToken>();

    public DbSet<IdentityUserProfile> IdentityUserProfiles => Set<IdentityUserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Align with TriVita unified EF migrations (single DB, module schemas). Without this, queries hit dbo.* while tables live in identity.*.
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
