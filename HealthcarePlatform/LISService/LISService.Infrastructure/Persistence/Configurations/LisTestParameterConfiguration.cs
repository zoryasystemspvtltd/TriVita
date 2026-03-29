using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisTestParameterConfiguration : IEntityTypeConfiguration<LisTestParameter>
{
    public void Configure(EntityTypeBuilder<LisTestParameter> builder)
    {
        builder.ToTable("LIS_TestParameters");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ParameterCode).HasMaxLength(100);
        builder.Property(e => e.ParameterName).HasMaxLength(300);
        builder.Property(e => e.ParameterNotes).HasMaxLength(500);
        builder.Property(e => e.EffectiveFrom).HasColumnType("date");
        builder.Property(e => e.EffectiveTo).HasColumnType("date");
    }
}