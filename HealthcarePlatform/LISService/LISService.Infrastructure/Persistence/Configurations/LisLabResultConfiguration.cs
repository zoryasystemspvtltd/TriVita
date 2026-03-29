using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisLabResultConfiguration : IEntityTypeConfiguration<LisLabResult>
{
    public void Configure(EntityTypeBuilder<LisLabResult> builder)
    {
        builder.ToTable("LIS_LabResults");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ResultNumeric).HasPrecision(18, 4);
        builder.Property(e => e.ResultText).HasColumnType("nvarchar(max)");
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}