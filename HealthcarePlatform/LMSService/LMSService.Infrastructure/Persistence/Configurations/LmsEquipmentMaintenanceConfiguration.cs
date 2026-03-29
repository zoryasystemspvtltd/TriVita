using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsEquipmentMaintenanceConfiguration : IEntityTypeConfiguration<LmsEquipmentMaintenance>
{
    public void Configure(EntityTypeBuilder<LmsEquipmentMaintenance> builder)
    {
        builder.ToTable("EquipmentMaintenance");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.MaintenanceNotes).HasColumnType("nvarchar(max)");
    }
}