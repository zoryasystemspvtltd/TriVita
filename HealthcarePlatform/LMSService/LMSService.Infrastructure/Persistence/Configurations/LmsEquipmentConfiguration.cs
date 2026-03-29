using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsEquipmentConfiguration : IEntityTypeConfiguration<LmsEquipment>
{
    public void Configure(EntityTypeBuilder<LmsEquipment> builder)
    {
        builder.ToTable("Equipment");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.EquipmentCode).HasMaxLength(80);
        builder.Property(e => e.EquipmentName).HasMaxLength(250);
        builder.Property(e => e.SerialNumber).HasMaxLength(120);
        builder.Property(e => e.EquipmentNotes).HasMaxLength(1000);
    }
}