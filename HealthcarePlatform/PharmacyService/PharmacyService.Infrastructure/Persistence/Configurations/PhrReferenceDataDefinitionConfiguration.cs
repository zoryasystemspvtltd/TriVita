using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrReferenceDataDefinitionConfiguration : IEntityTypeConfiguration<PhrReferenceDataDefinition>
{
    public void Configure(EntityTypeBuilder<PhrReferenceDataDefinition> builder)
    {
        builder.ToTable("ReferenceDataDefinition");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.DefinitionCode).HasMaxLength(150).IsRequired();
        builder.Property(e => e.DefinitionName).HasMaxLength(300).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
