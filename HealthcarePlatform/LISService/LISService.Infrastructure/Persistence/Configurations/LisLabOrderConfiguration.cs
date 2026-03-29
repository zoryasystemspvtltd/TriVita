using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisLabOrderConfiguration : IEntityTypeConfiguration<LisLabOrder>
{
    public void Configure(EntityTypeBuilder<LisLabOrder> builder)
    {
        builder.ToTable("LIS_LabOrder");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.LabOrderNo).HasMaxLength(60);
        builder.Property(e => e.ClinicalNotes).HasColumnType("nvarchar(max)");
    }
}