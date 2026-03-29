using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsVisitConfiguration : IEntityTypeConfiguration<HmsVisit>
{
    public void Configure(EntityTypeBuilder<HmsVisit> builder)
    {
        builder.ToTable("HMS_Visit");

        builder.HasKey(e => e.Id);

        builder.HasAlternateKey(e => new { e.TenantId, e.FacilityId, e.Id });

        builder.Property(e => e.VisitNo).HasMaxLength(60).IsRequired();
        builder.Property(e => e.ChiefComplaint).HasMaxLength(2000);

        builder.Property(e => e.FacilityId).IsRequired();

        builder.Property(e => e.RowVersion).IsRowVersion();

        builder.HasIndex(e => new { e.TenantId, e.FacilityId, e.VisitNo }).IsUnique();

        builder.HasOne(e => e.Appointment)
            .WithMany()
            .HasForeignKey(e => new { e.TenantId, e.FacilityId, e.AppointmentId })
            .HasPrincipalKey(a => new { a.TenantId, a.FacilityId, a.Id })
            .IsRequired(false);

        builder.HasOne(e => e.VisitType)
            .WithMany()
            .HasForeignKey(e => new { e.TenantId, e.VisitTypeId })
            .HasPrincipalKey(vt => new { vt.TenantId, vt.Id })
            .IsRequired();
    }
}
