using System.Net.Http.Json;
using CommunicationService.Application.Abstractions;
using CommunicationService.Application.Models;
using CommunicationService.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CommunicationService.Infrastructure.Services;

public sealed class SmsService : ISmsService
{
    private readonly HttpClient _http;
    private readonly CommunicationOptions _options;
    private readonly ILogger<SmsService> _logger;

    public SmsService(IHttpClientFactory httpClientFactory, IOptions<CommunicationOptions> options, ILogger<SmsService> logger)
    {
        _http = httpClientFactory.CreateClient("CommunicationSms");
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ChannelSendResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                to = message.ToPhoneNumber,
                body = message.Body,
                senderId = _options.Sms.SenderId,
                apiKey = _options.Sms.ApiKey
            };

            using var response = await _http.PostAsJsonAsync("", payload, cancellationToken);
            var text = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("SMS provider returned {Status}: {Body}", response.StatusCode, text);
                return ChannelSendResult.Fail($"SMS provider error: {(int)response.StatusCode}");
            }

            return ChannelSendResult.Ok(text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMS send failed.");
            return ChannelSendResult.Fail(ex.Message);
        }
    }
}
