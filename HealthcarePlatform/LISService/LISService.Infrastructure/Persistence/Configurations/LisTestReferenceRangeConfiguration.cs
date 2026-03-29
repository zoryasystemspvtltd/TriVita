using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisTestReferenceRangeConfiguration : IEntityTypeConfiguration<LisTestReferenceRange>
{
    public void Configure(EntityTypeBuilder<LisTestReferenceRange> builder)
    {
        builder.ToTable("LIS_TestReferenceRanges");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.MinValue).HasPrecision(18, 4);
        builder.Property(e => e.MaxValue).HasPrecision(18, 4);
        builder.Property(e => e.RangeNotes).HasMaxLength(800);
        builder.Property(e => e.EffectiveFromDate).HasColumnType("date");
        builder.Property(e => e.EffectiveToDate).HasColumnType("date");
        builder.Property(e => e.EffectiveFrom).HasColumnType("date");
        builder.Property(e => e.EffectiveTo).HasColumnType("date");
    }
}