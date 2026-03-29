using CommunicationService.Contracts.Notifications;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;

namespace CommunicationService.Application.Abstractions;

public interface INotificationService
{
    Task<BaseResponse<NotificationResponseDto>> CreateAsync(
        CreateNotificationRequestDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<NotificationResponseDto>> SendTemplateAsync(
        SendTemplateNotificationRequestDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<NotificationResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<PagedResponse<NotificationLogResponseDto>>> GetLogsPagedAsync(
        long? notificationChannelId,
        PagedQuery query,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<PagedResponse<NotificationTemplateResponseDto>>> GetTemplatesPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default);
}
