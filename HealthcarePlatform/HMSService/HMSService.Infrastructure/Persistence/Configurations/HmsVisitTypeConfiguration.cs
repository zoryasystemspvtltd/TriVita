using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsVisitTypeConfiguration : IEntityTypeConfiguration<HmsVisitType>
{
    public void Configure(EntityTypeBuilder<HmsVisitType> builder)
    {
        builder.ToTable("HMS_VisitType");

        builder.HasKey(e => e.Id);

        builder.HasAlternateKey(e => new { e.TenantId, e.Id });

        builder.Property(e => e.VisitTypeCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.VisitTypeName).HasMaxLength(250).IsRequired();

        builder.Property(e => e.EffectiveFrom).HasColumnType("date");
        builder.Property(e => e.EffectiveTo).HasColumnType("date");

        builder.Property(e => e.RowVersion).IsRowVersion();

        builder.HasIndex(e => new { e.TenantId, e.VisitTypeCode }).IsUnique();
    }
}
