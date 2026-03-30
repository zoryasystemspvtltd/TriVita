using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using HMSService.Application.Integration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HMSService.Infrastructure.Integration;

public sealed class LmsTestBookingClient : ILmsTestBookingClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LmsTestBookingClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public LmsTestBookingClient(
        HttpClient http,
        IHttpContextAccessor httpContextAccessor,
        ITenantContext tenant,
        ILogger<LmsTestBookingClient> logger)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
        _tenant = tenant;
        _logger = logger;
    }

    public async Task<BaseResponse<LmsTestBookingClientResponseDto>> CreateBookingAsync(
        CreateLmsTestBookingClientRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "api/v1/workflow/test-bookings")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        PropagateCallerContext(req);

        _logger.LogInformation(
            "HMS→LMS lab test booking create tenant {TenantId} facility {FacilityId} patient {PatientId}",
            _tenant.TenantId,
            _tenant.FacilityId,
            request.PatientId);

        using var response = await _http.SendAsync(req, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("LMS test-booking proxy failed {Status} body {Body}", response.StatusCode, body);
            return BaseResponse<LmsTestBookingClientResponseDto>.Fail(
                $"LMS returned {(int)response.StatusCode}: {response.ReasonPhrase}");
        }

        var parsed = JsonSerializer.Deserialize<BaseResponse<LmsTestBookingClientResponseDto>>(body, JsonOptions);
        if (parsed?.Data is { } data)
        {
            return BaseResponse<LmsTestBookingClientResponseDto>.Ok(data, parsed.Message);
        }

        var envelope = JsonSerializer.Deserialize<BaseResponse<JsonElement>>(body, JsonOptions);
        if (envelope?.Data.ValueKind == JsonValueKind.Object)
        {
            var d = envelope.Data;
            var dto = new LmsTestBookingClientResponseDto
            {
                Id = d.TryGetProperty("id", out var id) ? id.GetInt64() : 0,
                FacilityId = d.TryGetProperty("facilityId", out var f) ? f.GetInt64() : 0,
                BookingNo = d.TryGetProperty("bookingNo", out var bn) ? bn.GetString() ?? "" : "",
                PatientId = d.TryGetProperty("patientId", out var p) ? p.GetInt64() : 0,
                VisitId = d.TryGetProperty("visitId", out var v) && v.ValueKind == JsonValueKind.Number
                    ? v.GetInt64()
                    : null
            };
            return BaseResponse<LmsTestBookingClientResponseDto>.Ok(dto, envelope.Message);
        }

        return BaseResponse<LmsTestBookingClientResponseDto>.Fail("Unexpected LMS response shape.");
    }

    private void PropagateCallerContext(HttpRequestMessage request)
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null)
            return;

        if (ctx.Request.Headers.TryGetValue("Authorization", out var auth))
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(auth.ToString());

        request.Headers.TryAddWithoutValidation("X-Tenant-Id", _tenant.TenantId.ToString());
        if (_tenant.FacilityId is { } f)
            request.Headers.TryAddWithoutValidation("X-Facility-Id", f.ToString());
    }
}
