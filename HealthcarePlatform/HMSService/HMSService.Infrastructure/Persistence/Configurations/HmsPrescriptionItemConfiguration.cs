using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsPrescriptionItemConfiguration : IEntityTypeConfiguration<HmsPrescriptionItem>
{
    public void Configure(EntityTypeBuilder<HmsPrescriptionItem> builder)
    {
        builder.ToTable("HMS_PrescriptionItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Quantity).HasPrecision(18, 4);
        builder.Property(e => e.DosageText).HasMaxLength(200);
        builder.Property(e => e.FrequencyText).HasMaxLength(150);
        builder.Property(e => e.ItemNotes).HasMaxLength(1000);
    }
}