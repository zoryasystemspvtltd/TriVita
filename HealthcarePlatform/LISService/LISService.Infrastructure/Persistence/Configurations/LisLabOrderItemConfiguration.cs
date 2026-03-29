using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisLabOrderItemConfiguration : IEntityTypeConfiguration<LisLabOrderItem>
{
    public void Configure(EntityTypeBuilder<LisLabOrderItem> builder)
    {
        builder.ToTable("LIS_LabOrderItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.SpecimenVolume).HasPrecision(18, 4);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}