using CommunicationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationService.Infrastructure.Persistence.Configurations;

public sealed class ComNotificationChannelConfiguration : IEntityTypeConfiguration<ComNotificationChannel>
{
    public void Configure(EntityTypeBuilder<ComNotificationChannel> builder)
    {
        builder.ToTable("COM_NotificationChannel");
        builder.Property(x => x.ErrorMessage).HasMaxLength(1000);
        builder.Property(x => x.FacilityId).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasOne(c => c.Notification)
            .WithMany(n => n.Channels)
            .HasForeignKey(c => new { c.TenantId, c.FacilityId, c.NotificationId })
            .HasPrincipalKey(n => new { n.TenantId, n.FacilityId, n.Id });

        builder.HasOne(c => c.Template)
            .WithMany(t => t.Channels)
            .HasForeignKey(c => new { c.TenantId, c.FacilityId, c.TemplateId })
            .HasPrincipalKey(t => new { t.TenantId, t.FacilityId, t.Id })
            .IsRequired(false);
    }
}
