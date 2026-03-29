using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisReportLockStateConfiguration : IEntityTypeConfiguration<LisReportLockState>
{
    public void Configure(EntityTypeBuilder<LisReportLockState> builder)
    {
        builder.ToTable("LIS_ReportLockState");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}
