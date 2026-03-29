using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisSampleTrackingConfiguration : IEntityTypeConfiguration<LisSampleTracking>
{
    public void Configure(EntityTypeBuilder<LisSampleTracking> builder)
    {
        builder.ToTable("LIS_SampleTracking");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.TrackingNo).HasMaxLength(120);
        builder.Property(e => e.TrackingNotes).HasMaxLength(1000);
    }
}