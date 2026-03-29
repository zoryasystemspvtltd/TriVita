using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisTestParameterProfileConfiguration : IEntityTypeConfiguration<LisTestParameterProfile>
{
    public void Configure(EntityTypeBuilder<LisTestParameterProfile> builder)
    {
        builder.ToTable("LIS_TestParameterProfile");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.AnalyzerChannelCode).HasMaxLength(80);
        builder.Property(e => e.LoincCode).HasMaxLength(40);
        builder.Property(e => e.Notes).HasMaxLength(500);
    }
}
