using PharmacyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PharmacyService.Infrastructure.Persistence.Configurations;

public sealed class PhrUnitConfiguration : IEntityTypeConfiguration<PhrUnit>
{
    public void Configure(EntityTypeBuilder<PhrUnit> builder)
    {
        builder.ToTable("Unit");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.UnitCode).HasMaxLength(80).IsRequired();
        builder.Property(e => e.UnitName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.UnitSymbol).HasColumnName("UnitType").HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
