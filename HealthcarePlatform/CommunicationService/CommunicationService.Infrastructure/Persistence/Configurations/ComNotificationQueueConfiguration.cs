using CommunicationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationService.Infrastructure.Persistence.Configurations;

public sealed class ComNotificationQueueConfiguration : IEntityTypeConfiguration<ComNotificationQueue>
{
    public void Configure(EntityTypeBuilder<ComNotificationQueue> builder)
    {
        builder.ToTable("COM_NotificationQueue");
        builder.Property(x => x.FacilityId).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(q => q.Notification)
            .WithMany(n => n.Queues)
            .HasForeignKey(q => new { q.TenantId, q.FacilityId, q.NotificationId })
            .HasPrincipalKey(n => new { n.TenantId, n.FacilityId, n.Id });
    }
}
