namespace CommunicationService.Contracts.Notifications;

/// <summary>HTTP client contract for other microservices (no direct SMTP/SMS calls outside CommunicationService).</summary>
public interface INotificationApiClient
{
    Task<NotificationResponseDto?> CreateNotificationAsync(
        CreateNotificationRequestDto request,
        CancellationToken cancellationToken = default);
}
