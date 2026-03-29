using System.Linq;
using CommunicationService.Domain.Entities;
using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace CommunicationService.Infrastructure.Persistence;

public sealed class CommunicationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public CommunicationDbContext(DbContextOptions<CommunicationDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<ComNotification> Notifications => Set<ComNotification>();

    public DbSet<ComNotificationRecipient> NotificationRecipients => Set<ComNotificationRecipient>();

    public DbSet<ComNotificationChannel> NotificationChannels => Set<ComNotificationChannel>();

    public DbSet<ComNotificationTemplate> NotificationTemplates => Set<ComNotificationTemplate>();

    public DbSet<ComNotificationLog> NotificationLogs => Set<ComNotificationLog>();

    public DbSet<ComNotificationQueue> NotificationQueues => Set<ComNotificationQueue>();

    public long TenantFilter => _tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommunicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            if (typeof(BaseEntity).IsAssignableFrom(clr) && clr != typeof(BaseEntity))
            {
                var configureMethod = typeof(CommunicationDbContext)
                    .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .First(m => m.Name == nameof(ConfigureTenantFilter) && m.IsGenericMethodDefinition);

                configureMethod.MakeGenericMethod(clr).Invoke(null, new object[] { modelBuilder, this });
            }
        }
    }

    private static void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder, CommunicationDbContext ctx)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == ctx.TenantFilter);
    }
}
