using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsAppointmentStatusHistoryConfiguration : IEntityTypeConfiguration<HmsAppointmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<HmsAppointmentStatusHistory> builder)
    {
        builder.ToTable("HMS_AppointmentStatusHistory");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.StatusNote).HasMaxLength(1000);
    }
}