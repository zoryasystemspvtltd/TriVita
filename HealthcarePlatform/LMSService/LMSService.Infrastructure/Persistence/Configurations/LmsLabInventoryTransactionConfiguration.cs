using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsLabInventoryTransactionConfiguration : IEntityTypeConfiguration<LmsLabInventoryTransaction>
{
    public void Configure(EntityTypeBuilder<LmsLabInventoryTransaction> builder)
    {
        builder.ToTable("LabInventoryTransactions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QuantityDelta).HasPrecision(18, 4);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}