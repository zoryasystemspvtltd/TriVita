using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsAppointmentQueueConfiguration : IEntityTypeConfiguration<HmsAppointmentQueue>
{
    public void Configure(EntityTypeBuilder<HmsAppointmentQueue> builder)
    {
        builder.ToTable("HMS_AppointmentQueue");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.QueueToken).HasMaxLength(60);
    }
}