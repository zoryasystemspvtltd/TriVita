using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsQcRecordConfiguration : IEntityTypeConfiguration<LmsQcRecord>
{
    public void Configure(EntityTypeBuilder<LmsQcRecord> builder)
    {
        builder.ToTable("QCRecords");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.LotNo).HasMaxLength(120);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}