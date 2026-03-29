using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsQcResultConfiguration : IEntityTypeConfiguration<LmsQcResult>
{
    public void Configure(EntityTypeBuilder<LmsQcResult> builder)
    {
        builder.ToTable("QCResults");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ResultNumeric).HasPrecision(18, 4);
        builder.Property(e => e.ResultText).HasColumnType("nvarchar(max)");
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}