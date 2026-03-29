using CommunicationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationService.Infrastructure.Persistence.Configurations;

public sealed class ComNotificationLogConfiguration : IEntityTypeConfiguration<ComNotificationLog>
{
    public void Configure(EntityTypeBuilder<ComNotificationLog> builder)
    {
        builder.ToTable("COM_NotificationLog");
        builder.Property(x => x.FacilityId).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(l => l.NotificationChannel)
            .WithMany(c => c.Logs)
            .HasForeignKey(l => new { l.TenantId, l.FacilityId, l.NotificationChannelId })
            .HasPrincipalKey(c => new { c.TenantId, c.FacilityId, c.Id });
    }
}
