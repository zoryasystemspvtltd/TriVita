using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisReportDetailConfiguration : IEntityTypeConfiguration<LisReportDetail>
{
    public void Configure(EntityTypeBuilder<LisReportDetail> builder)
    {
        builder.ToTable("LIS_ReportDetails");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ResultDisplayText).HasColumnType("nvarchar(max)");
        builder.Property(e => e.ReferenceRangeDisplayText).HasColumnType("nvarchar(max)");
        builder.Property(e => e.LineNotes).HasColumnType("nvarchar(max)");
    }
}