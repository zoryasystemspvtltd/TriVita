using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsAppointmentConfiguration : IEntityTypeConfiguration<HmsAppointment>
{
    public void Configure(EntityTypeBuilder<HmsAppointment> builder)
    {
        builder.ToTable("HMS_Appointment");

        builder.HasKey(e => e.Id);

        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });

        builder.Property(e => e.AppointmentNo).HasMaxLength(60).IsRequired();
        builder.Property(e => e.Reason).HasMaxLength(1000);

        builder.Property(e => e.FacilityId).IsRequired();

        builder.Property(e => e.EffectiveFrom).HasColumnType("date");
        builder.Property(e => e.EffectiveTo).HasColumnType("date");

        builder.Property(e => e.RowVersion).IsRowVersion();

        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.AppointmentNo }).IsUnique();

        builder.HasOne(e => e.VisitType)
            .WithMany()
            .HasForeignKey(e => new { e.TenantId, e.VisitTypeId })
            .HasPrincipalKey(vt => new { vt.TenantId, vt.Id })
            .IsRequired(false);
    }
}
