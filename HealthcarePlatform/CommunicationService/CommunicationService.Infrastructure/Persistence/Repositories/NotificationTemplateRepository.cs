using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CommunicationService.Infrastructure.Persistence.Repositories;

public sealed class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly CommunicationDbContext _db;

    public NotificationTemplateRepository(CommunicationDbContext db)
    {
        _db = db;
    }

    public async Task<ComNotificationTemplate?> GetByCodeAndChannelAsync(
        long facilityId,
        string templateCode,
        long channelTypeReferenceValueId,
        CancellationToken cancellationToken = default)
    {
        return await _db.NotificationTemplates
            .Where(t => t.FacilityId == facilityId
                        && t.TemplateCode == templateCode
                        && t.ChannelTypeReferenceValueId == channelTypeReferenceValueId
                        && t.IsActive)
            .OrderByDescending(t => t.Version)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<ComNotificationTemplate> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _db.NotificationTemplates.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(t =>
                t.TemplateCode.Contains(s) || t.TemplateName.Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(t => t.TemplateCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
