using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrReferenceDataValueConfiguration : IEntityTypeConfiguration<PhrReferenceDataValue>
{
    public void Configure(EntityTypeBuilder<PhrReferenceDataValue> builder)
    {
        builder.ToTable("ReferenceDataValue");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ReferenceDataDefinitionId).IsRequired();
        builder.Property(e => e.ValueCode).HasMaxLength(150).IsRequired();
        builder.Property(e => e.ValueName).HasMaxLength(300).IsRequired();
        builder.Property(e => e.ValueText).HasMaxLength(1000);
        builder.Property(e => e.SortOrder).IsRequired();
    }
}
