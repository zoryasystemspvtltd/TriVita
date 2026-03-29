using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrMedicineCategoryConfiguration : IEntityTypeConfiguration<PhrMedicineCategory>
{
    public void Configure(EntityTypeBuilder<PhrMedicineCategory> builder)
    {
        builder.ToTable("MedicineCategory");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CategoryCode).HasMaxLength(80);
        builder.Property(e => e.CategoryName).HasMaxLength(250);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.EffectiveFrom).HasColumnType("date");
        builder.Property(e => e.EffectiveTo).HasColumnType("date");
    }
}