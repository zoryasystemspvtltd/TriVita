using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisResultHistoryConfiguration : IEntityTypeConfiguration<LisResultHistory>
{
    public void Configure(EntityTypeBuilder<LisResultHistory> builder)
    {
        builder.ToTable("LIS_ResultHistory");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.SnapshotResultNumeric).HasPrecision(18, 4);
        builder.Property(e => e.SnapshotResultText).HasColumnType("nvarchar(max)");
        builder.Property(e => e.ChangeReason).HasMaxLength(1000);
    }
}