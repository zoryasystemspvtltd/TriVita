using CommunicationService.Application.Models;

namespace CommunicationService.Application.Abstractions;

public interface IEmailService
{
    Task<ChannelSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
