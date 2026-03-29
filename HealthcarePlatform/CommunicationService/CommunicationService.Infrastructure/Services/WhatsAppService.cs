using System.Net.Http.Json;
using CommunicationService.Application.Abstractions;
using CommunicationService.Application.Models;
using CommunicationService.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CommunicationService.Infrastructure.Services;

public sealed class WhatsAppService : IWhatsAppService
{
    private readonly HttpClient _http;
    private readonly CommunicationOptions _options;
    private readonly ILogger<WhatsAppService> _logger;

    public WhatsAppService(IHttpClientFactory httpClientFactory, IOptions<CommunicationOptions> options, ILogger<WhatsAppService> logger)
    {
        _http = httpClientFactory.CreateClient("CommunicationWhatsApp");
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ChannelSendResult> SendAsync(WhatsAppMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                to = message.ToPhoneNumber,
                body = message.Body,
                from = _options.WhatsApp.FromNumber,
                apiKey = _options.WhatsApp.ApiKey
            };

            using var response = await _http.PostAsJsonAsync("", payload, cancellationToken);
            var text = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("WhatsApp provider returned {Status}: {Body}", response.StatusCode, text);
                return ChannelSendResult.Fail($"WhatsApp provider error: {(int)response.StatusCode}");
            }

            return ChannelSendResult.Ok(text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WhatsApp send failed.");
            return ChannelSendResult.Fail(ex.Message);
        }
    }
}
