using CommunicationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunicationService.Infrastructure.Persistence.Configurations;

public sealed class ComNotificationTemplateConfiguration : IEntityTypeConfiguration<ComNotificationTemplate>
{
    public void Configure(EntityTypeBuilder<ComNotificationTemplate> builder)
    {
        builder.ToTable("COM_NotificationTemplate");
        builder.Property(x => x.TemplateCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.TemplateName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.BodyTemplate).IsRequired();
        builder.Property(x => x.FacilityId).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}
