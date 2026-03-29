using CommunicationService.Application.Models;

namespace CommunicationService.Application.Abstractions;

public interface IWhatsAppService
{
    Task<ChannelSendResult> SendAsync(WhatsAppMessage message, CancellationToken cancellationToken = default);
}
