using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisSampleLifecycleEventConfiguration : IEntityTypeConfiguration<LisSampleLifecycleEvent>
{
    public void Configure(EntityTypeBuilder<LisSampleLifecycleEvent> builder)
    {
        builder.ToTable("LIS_SampleLifecycleEvent");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.EventNotes).HasMaxLength(1000);
    }
}
