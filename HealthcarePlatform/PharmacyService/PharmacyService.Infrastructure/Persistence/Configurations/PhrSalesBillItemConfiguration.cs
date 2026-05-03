using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrSalesBillItemConfiguration : IEntityTypeConfiguration<PhrSalesBillItem>
{
    public void Configure(EntityTypeBuilder<PhrSalesBillItem> builder)
    {
        builder.ToTable("SalesBillItems");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();

        builder.Property(e => e.Quantity).HasPrecision(18, 4);
        builder.Property(e => e.UnitPrice).HasPrecision(18, 4);
        builder.Property(e => e.LineTotal).HasPrecision(18, 4);

        builder.HasOne<PhrSalesBill>()
            .WithMany()
            .HasForeignKey(e => e.SalesBillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PhrMedicine>()
            .WithMany()
            .HasForeignKey(e => e.MedicineId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<PhrMedicineBatch>()
            .WithMany()
            .HasForeignKey(e => e.MedicineBatchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
