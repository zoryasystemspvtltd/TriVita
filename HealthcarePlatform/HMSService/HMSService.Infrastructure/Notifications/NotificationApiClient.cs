using System.Net.Http.Json;
using System.Text.Json;
using CommunicationService.Contracts.Notifications;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Http;

namespace HMSService.Infrastructure.Notifications;

public sealed class NotificationApiClient : INotificationApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantContext _tenant;

    public NotificationApiClient(
        HttpClient http,
        IHttpContextAccessor httpContextAccessor,
        ITenantContext tenant)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
        _tenant = tenant;
    }

    public async Task<NotificationResponseDto?> CreateNotificationAsync(
        CreateNotificationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "api/v1/notifications")
        {
            Content = JsonContent.Create(request)
        };

        var auth = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrEmpty(auth))
            req.Headers.TryAddWithoutValidation("Authorization", auth);

        req.Headers.TryAddWithoutValidation("X-Tenant-Id", _tenant.TenantId.ToString());
        if (_tenant.FacilityId is { } f)
            req.Headers.TryAddWithoutValidation("X-Facility-Id", f.ToString());

        using var response = await _http.SendAsync(req, cancellationToken);
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var envelope = await JsonSerializer.DeserializeAsync<BaseResponse<NotificationResponseDto>>(stream, JsonOptions, cancellationToken);
        return envelope?.Data;
    }
}
