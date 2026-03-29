using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsPrescriptionConfiguration : IEntityTypeConfiguration<HmsPrescription>
{
    public void Configure(EntityTypeBuilder<HmsPrescription> builder)
    {
        builder.ToTable("HMS_Prescription");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PrescriptionNo).HasMaxLength(60);
        builder.Property(e => e.Indication).HasMaxLength(1000);
        builder.Property(e => e.Notes).HasColumnType("nvarchar(max)");
    }
}