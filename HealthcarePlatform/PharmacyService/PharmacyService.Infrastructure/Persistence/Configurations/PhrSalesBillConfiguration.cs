using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrSalesBillConfiguration : IEntityTypeConfiguration<PhrSalesBill>
{
    public void Configure(EntityTypeBuilder<PhrSalesBill> builder)
    {
        builder.ToTable("SalesBill");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BillNo).HasMaxLength(60);
        builder.Property(e => e.Notes).HasMaxLength(1000);

        builder.Property(e => e.SubTotal).HasPrecision(18, 4);
        builder.Property(e => e.DiscountAmount).HasPrecision(18, 4);
        builder.Property(e => e.GstPercent).HasPrecision(18, 4);
        builder.Property(e => e.GstAmount).HasPrecision(18, 4);
        builder.Property(e => e.OtherTaxAmount).HasPrecision(18, 4);
        builder.Property(e => e.NetAmount).HasPrecision(18, 4);

        builder.Property(e => e.Status).HasConversion<int>();

        builder.HasOne<PhrCustomer>()
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
