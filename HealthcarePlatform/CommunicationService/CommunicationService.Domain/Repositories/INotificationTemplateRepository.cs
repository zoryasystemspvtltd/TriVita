using CommunicationService.Domain.Entities;

namespace CommunicationService.Domain.Repositories;

public interface INotificationTemplateRepository
{
    Task<ComNotificationTemplate?> GetByCodeAndChannelAsync(
        long facilityId,
        string templateCode,
        long channelTypeReferenceValueId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ComNotificationTemplate> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);
}
