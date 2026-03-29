using System.Net.Http.Json;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Healthcare.Common.Integration.SharedService;

/// <summary>HTTP client for SharedService facility hierarchy validation (forwards Bearer and X-Tenant-Id).</summary>
public sealed class SharedEnterpriseApiClient : IFacilityTenantValidator
{
    private readonly HttpClient _http;
    private readonly IOptions<SharedServiceClientOptions> _options;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly ILogger<SharedEnterpriseApiClient> _logger;

    public SharedEnterpriseApiClient(
        HttpClient http,
        IOptions<SharedServiceClientOptions> options,
        IHttpContextAccessor? httpContextAccessor,
        ILogger<SharedEnterpriseApiClient> logger)
    {
        _http = http;
        _options = options;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<FacilityHierarchyContext?> GetFacilityContextAsync(
        long tenantId,
        long facilityId,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Value.ValidateFacilityWithSharedService)
        {
            _logger.LogDebug(
                "Skipping SharedService facility validation (disabled) TenantId={TenantId} FacilityId={FacilityId}",
                tenantId,
                facilityId);
            return new FacilityHierarchyContext
            {
                TenantId = tenantId,
                FacilityId = facilityId
            };
        }

        try
        {
            var path = $"api/v1.0/facilities/{facilityId}/hierarchy-context";
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.TryAddWithoutValidation("X-Tenant-Id", tenantId.ToString());

            var auth = _httpContextAccessor?.HttpContext?.Request.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(auth))
                request.Headers.TryAddWithoutValidation("Authorization", auth);

            var response = await _http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "SharedService facility validation failed HTTP {Status} TenantId={TenantId} FacilityId={FacilityId}",
                    (int)response.StatusCode,
                    tenantId,
                    facilityId);
                return null;
            }

            var envelope = await response.Content.ReadFromJsonAsync<BaseResponse<FacilityHierarchyContext>>(
                cancellationToken: cancellationToken);

            if (envelope is not { Success: true, Data: { } data })
            {
                _logger.LogWarning(
                    "SharedService facility validation empty payload TenantId={TenantId} FacilityId={FacilityId}",
                    tenantId,
                    facilityId);
                return null;
            }

            _logger.LogInformation(
                "Facility validated via SharedService TenantId={TenantId} FacilityId={FacilityId} EnterpriseId={EnterpriseId}",
                tenantId,
                facilityId,
                data.EnterpriseId);

            return data;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(
                ex,
                "SharedService facility validation error TenantId={TenantId} FacilityId={FacilityId}",
                tenantId,
                facilityId);
            return null;
        }
    }
}
