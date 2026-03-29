using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.Integration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace HMSService.Infrastructure.Integration;

public sealed class LmsBillingClient : ILmsBillingClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LmsBillingClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public LmsBillingClient(
        HttpClient http,
        IHttpContextAccessor httpContextAccessor,
        ITenantContext tenant,
        ILogger<LmsBillingClient> logger)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
        _tenant = tenant;
        _logger = logger;
    }

    public async Task<BaseResponse<PagedResponse<LabInvoiceSummaryDto>>> GetLabInvoicesPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var url = $"api/v1/lab-invoice-headers?Page={query.Page}&PageSize={query.PageSize}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        PropagateCallerContext(request);

        _logger.LogInformation(
            "LMS billing proxy: GET lab invoices tenant {TenantId} facility {FacilityId}",
            _tenant.TenantId,
            _tenant.FacilityId);

        using var response = await _http.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("LMS billing proxy failed {Status} body {Body}", response.StatusCode, body);
            return BaseResponse<PagedResponse<LabInvoiceSummaryDto>>.Fail(
                $"LMS returned {(int)response.StatusCode}: {response.ReasonPhrase}");
        }

        var parsed = JsonSerializer.Deserialize<BaseResponse<PagedResponse<LabInvoiceSummaryDto>>>(body, JsonOptions);
        return parsed ?? BaseResponse<PagedResponse<LabInvoiceSummaryDto>>.Fail("Empty LMS response.");
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
