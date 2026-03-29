using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrMedicineCompositionConfiguration : IEntityTypeConfiguration<PhrMedicineComposition>
{
    public void Configure(EntityTypeBuilder<PhrMedicineComposition> builder)
    {
        builder.ToTable("MedicineComposition");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Amount).HasPrecision(18, 4);
    }
}