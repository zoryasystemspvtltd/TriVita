namespace CommunicationService.Application.Models;

public sealed class ChannelSendResult
{
    public bool Success { get; init; }

    public string? ResponseText { get; init; }

    public string? Error { get; init; }

    public static ChannelSendResult Ok(string? response = null) =>
        new() { Success = true, ResponseText = response };

    public static ChannelSendResult Fail(string error) =>
        new() { Success = false, Error = error };
}
