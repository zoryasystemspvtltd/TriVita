using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisSampleTypeConfiguration : IEntityTypeConfiguration<LisSampleType>
{
    public void Configure(EntityTypeBuilder<LisSampleType> builder)
    {
        builder.ToTable("LIS_SampleType");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.SampleTypeCode).HasMaxLength(80);
        builder.Property(e => e.SampleTypeName).HasMaxLength(250);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.EffectiveFrom).HasColumnType("date");
        builder.Property(e => e.EffectiveTo).HasColumnType("date");
    }
}