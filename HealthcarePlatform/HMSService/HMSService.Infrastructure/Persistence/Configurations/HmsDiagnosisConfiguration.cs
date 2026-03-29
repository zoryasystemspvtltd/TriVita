using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsDiagnosisConfiguration : IEntityTypeConfiguration<HmsDiagnosis>
{
    public void Configure(EntityTypeBuilder<HmsDiagnosis> builder)
    {
        builder.ToTable("HMS_Diagnosis");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ICDSystem).HasMaxLength(30);
        builder.Property(e => e.ICDCode).HasMaxLength(30);
        builder.Property(e => e.ICDVersion).HasMaxLength(20);
        builder.Property(e => e.ICDDescription).HasMaxLength(500);
        builder.Property(e => e.DiagnosisOn).HasColumnType("date");
    }
}