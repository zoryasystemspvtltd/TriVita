using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisTestMasterConfiguration : IEntityTypeConfiguration<LisTestMaster>
{
    public void Configure(EntityTypeBuilder<LisTestMaster> builder)
    {
        builder.ToTable("LIS_TestMaster");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.TestCode).HasMaxLength(80);
        builder.Property(e => e.TestName).HasMaxLength(250);
        builder.Property(e => e.TestDescription).HasMaxLength(1000);
        builder.Property(e => e.EffectiveFrom).HasColumnType("date");
        builder.Property(e => e.EffectiveTo).HasColumnType("date");
    }
}