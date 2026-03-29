namespace CommunicationService.Application.Models;

public sealed class WhatsAppMessage
{
    public string ToPhoneNumber { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;
}
