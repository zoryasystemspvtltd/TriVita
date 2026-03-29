using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsEquipmentCalibrationConfiguration : IEntityTypeConfiguration<LmsEquipmentCalibration>
{
    public void Configure(EntityTypeBuilder<LmsEquipmentCalibration> builder)
    {
        builder.ToTable("EquipmentCalibration");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ResultNumeric).HasPrecision(18, 4);
        builder.Property(e => e.ResultText).HasMaxLength(1000);
        builder.Property(e => e.Comments).HasColumnType("nvarchar(max)");
    }
}