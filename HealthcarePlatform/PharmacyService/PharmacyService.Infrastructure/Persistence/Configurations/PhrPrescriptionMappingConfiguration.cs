using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrPrescriptionMappingConfiguration : IEntityTypeConfiguration<PhrPrescriptionMapping>
{
    public void Configure(EntityTypeBuilder<PhrPrescriptionMapping> builder)
    {
        builder.ToTable("PrescriptionMapping");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.MappedQty).HasPrecision(18, 4);
        builder.Property(e => e.MappingNotes).HasMaxLength(1000);
    }
}