using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisTestCategoryConfiguration : IEntityTypeConfiguration<LisTestCategory>
{
    public void Configure(EntityTypeBuilder<LisTestCategory> builder)
    {
        builder.ToTable("LIS_TestCategory");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CategoryCode).HasMaxLength(80);
        builder.Property(e => e.CategoryName).HasMaxLength(250);
        builder.Property(e => e.EffectiveFrom).HasColumnType("date");
        builder.Property(e => e.EffectiveTo).HasColumnType("date");
    }
}