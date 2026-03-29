using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrStockTransferConfiguration : IEntityTypeConfiguration<PhrStockTransfer>
{
    public void Configure(EntityTypeBuilder<PhrStockTransfer> builder)
    {
        builder.ToTable("StockTransfer");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.TransferNo).HasMaxLength(60);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}