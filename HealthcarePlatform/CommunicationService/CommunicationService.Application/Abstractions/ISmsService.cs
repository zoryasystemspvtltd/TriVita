using CommunicationService.Application.Models;

namespace CommunicationService.Application.Abstractions;

public interface ISmsService
{
    Task<ChannelSendResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default);
}
