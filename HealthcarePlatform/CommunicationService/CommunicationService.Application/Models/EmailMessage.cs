namespace CommunicationService.Application.Models;

public sealed class EmailMessage
{
    public IReadOnlyList<string> ToAddresses { get; init; } = Array.Empty<string>();

    public string Subject { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public bool IsHtml { get; init; }
}
