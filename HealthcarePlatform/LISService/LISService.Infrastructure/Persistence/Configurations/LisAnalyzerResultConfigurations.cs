using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisAnalyzerResultHeaderConfiguration : IEntityTypeConfiguration<LisAnalyzerResultHeader>
{
    public void Configure(EntityTypeBuilder<LisAnalyzerResultHeader> builder)
    {
        builder.ToTable("LIS_AnalyzerResultHeader");
        builder.HasKey(e => e.Id);
        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.BarcodeValue).HasMaxLength(120);
        builder.Property(e => e.EquipmentAssayCode).HasMaxLength(120);
        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.BarcodeValue });
    }
}

public sealed class LisAnalyzerResultLineConfiguration : IEntityTypeConfiguration<LisAnalyzerResultLine>
{
    public void Configure(EntityTypeBuilder<LisAnalyzerResultLine> builder)
    {
        builder.ToTable("LIS_AnalyzerResultLine");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.EquipmentResultCode).HasMaxLength(120);
        builder.Property(e => e.ResultText).HasMaxLength(2000);
        builder.HasOne<LisAnalyzerResultHeader>().WithMany().HasForeignKey(e => new { e.TenantId, e.FacilityId, e.AnalyzerResultHeaderId })
            .HasPrincipalKey(h => new { h.TenantId, h.FacilityId, h.Id });
    }
}
