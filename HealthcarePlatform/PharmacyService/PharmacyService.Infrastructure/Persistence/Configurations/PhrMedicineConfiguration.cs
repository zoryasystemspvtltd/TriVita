using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrMedicineConfiguration : IEntityTypeConfiguration<PhrMedicine>
{
    public void Configure(EntityTypeBuilder<PhrMedicine> builder)
    {
        builder.ToTable("Medicine");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.MedicineCode).HasMaxLength(80);
        builder.Property(e => e.MedicineName).HasMaxLength(300);
        builder.Property(e => e.Strength).HasMaxLength(120);
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.Property(e => e.PrimaryCompositionId);
        builder
            .HasOne<PhrComposition>()
            .WithMany()
            .HasForeignKey(e => e.PrimaryCompositionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}