using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsProcessingStageConfiguration : IEntityTypeConfiguration<LmsProcessingStage>
{
    public void Configure(EntityTypeBuilder<LmsProcessingStage> builder)
    {
        builder.ToTable("ProcessingStages");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.StageCode).HasMaxLength(80);
        builder.Property(e => e.StageName).HasMaxLength(250);
        builder.Property(e => e.StageNotes).HasMaxLength(500);
    }
}