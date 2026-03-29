using CommunicationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationService.Infrastructure.Persistence.Configurations;

public sealed class ComNotificationRecipientConfiguration : IEntityTypeConfiguration<ComNotificationRecipient>
{
    public void Configure(EntityTypeBuilder<ComNotificationRecipient> builder)
    {
        builder.ToTable("COM_NotificationRecipient");
        builder.Property(x => x.Email).HasMaxLength(300);
        builder.Property(x => x.PhoneNumber).HasMaxLength(50);
        builder.Property(x => x.FacilityId).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(r => r.Notification)
            .WithMany(n => n.Recipients)
            .HasForeignKey(r => new { r.TenantId, r.FacilityId, r.NotificationId })
            .HasPrincipalKey(n => new { n.TenantId, n.FacilityId, n.Id });
    }
}
