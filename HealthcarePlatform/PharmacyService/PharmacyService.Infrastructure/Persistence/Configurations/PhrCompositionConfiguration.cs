using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrCompositionConfiguration : IEntityTypeConfiguration<PhrComposition>
{
    public void Configure(EntityTypeBuilder<PhrComposition> builder)
    {
        builder.ToTable("Composition");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CompositionName).HasMaxLength(250);
        builder.Property(e => e.CompositionCode).HasMaxLength(120);
        builder.Property(e => e.Notes).HasMaxLength(500);
    }
}