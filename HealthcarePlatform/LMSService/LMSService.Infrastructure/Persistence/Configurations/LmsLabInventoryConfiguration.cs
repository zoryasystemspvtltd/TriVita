using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsLabInventoryConfiguration : IEntityTypeConfiguration<LmsLabInventory>
{
    public void Configure(EntityTypeBuilder<LmsLabInventory> builder)
    {
        builder.ToTable("LabInventory");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.InventoryItemCode).HasMaxLength(80);
        builder.Property(e => e.InventoryItemName).HasMaxLength(250);
        builder.Property(e => e.CurrentQty).HasPrecision(18, 4);
    }
}