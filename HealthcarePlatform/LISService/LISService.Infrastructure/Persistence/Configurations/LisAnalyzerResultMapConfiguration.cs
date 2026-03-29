using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisAnalyzerResultMapConfiguration : IEntityTypeConfiguration<LisAnalyzerResultMap>
{
    public void Configure(EntityTypeBuilder<LisAnalyzerResultMap> builder)
    {
        builder.ToTable("LIS_AnalyzerResultMap");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ExternalTestCode).HasMaxLength(80);
        builder.Property(e => e.ExternalParameterCode).HasMaxLength(120);
    }
}
