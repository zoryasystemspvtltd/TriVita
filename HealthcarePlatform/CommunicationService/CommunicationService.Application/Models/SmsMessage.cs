namespace CommunicationService.Application.Models;

public sealed class SmsMessage
{
    public string ToPhoneNumber { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;
}
