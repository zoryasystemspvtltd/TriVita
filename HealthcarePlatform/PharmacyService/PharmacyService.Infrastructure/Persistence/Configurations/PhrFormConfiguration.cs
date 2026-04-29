using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyService.Domain.Entities;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrFormConfiguration : IEntityTypeConfiguration<PhrForm>
{
    public void Configure(EntityTypeBuilder<PhrForm> builder)
    {
        builder.ToTable("Form");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();

        builder.Property(e => e.FormCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.FormName).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1000);

        builder.HasIndex(e => new { e.TenantId, e.FormCode }).IsUnique();
        builder.HasIndex(e => new { e.TenantId, e.FormName }).IsUnique();
    }
}
