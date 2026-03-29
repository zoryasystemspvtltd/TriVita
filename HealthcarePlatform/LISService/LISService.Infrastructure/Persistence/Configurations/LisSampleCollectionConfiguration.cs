using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisSampleCollectionConfiguration : IEntityTypeConfiguration<LisSampleCollection>
{
    public void Configure(EntityTypeBuilder<LisSampleCollection> builder)
    {
        builder.ToTable("LIS_SampleCollection");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CollectedQuantity).HasPrecision(18, 4);
        builder.Property(e => e.CollectionNotes).HasMaxLength(1000);
    }
}