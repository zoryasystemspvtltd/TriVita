using CommunicationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationService.Infrastructure.Persistence.Configurations;

public sealed class ComNotificationConfiguration : IEntityTypeConfiguration<ComNotification>
{
    public void Configure(EntityTypeBuilder<ComNotification> builder)
    {
        builder.ToTable("COM_Notification");
        builder.Property(x => x.EventType).HasMaxLength(150).IsRequired();
        builder.Property(x => x.FacilityId).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}
