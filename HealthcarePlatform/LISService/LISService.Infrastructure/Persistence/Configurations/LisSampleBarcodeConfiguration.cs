using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisSampleBarcodeConfiguration : IEntityTypeConfiguration<LisSampleBarcode>
{
    public void Configure(EntityTypeBuilder<LisSampleBarcode> builder)
    {
        builder.ToTable("LIS_SampleBarcode");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BarcodeValue).HasMaxLength(120);
    }
}
