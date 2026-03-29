using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.Infrastructure.Persistence.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Identity_Users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Email).HasMaxLength(320).IsRequired();
        builder.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();

        builder.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Role).HasMaxLength(128).IsRequired();
        builder.Property(e => e.TenantId).IsRequired();
        builder.Property(e => e.IsActive).IsRequired();
    }
}
