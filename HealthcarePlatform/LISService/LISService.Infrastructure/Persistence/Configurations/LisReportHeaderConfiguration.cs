using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisReportHeaderConfiguration : IEntityTypeConfiguration<LisReportHeader>
{
    public void Configure(EntityTypeBuilder<LisReportHeader> builder)
    {
        builder.ToTable("LIS_ReportHeader");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ReportNo).HasMaxLength(60);
    }
}