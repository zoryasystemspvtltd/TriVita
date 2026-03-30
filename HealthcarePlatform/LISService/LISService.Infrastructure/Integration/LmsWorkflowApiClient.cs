using System.Net.Http.Json;
using System.Text.Json;
using Healthcare.Common.Responses;
using LISService.Application.Abstractions;
using LISService.Application.DTOs.Analyzer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LISService.Infrastructure.Integration;

public sealed class LmsWorkflowApiClient : ILmsWorkflowApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<LmsWorkflowApiClient> _logger;

    public LmsWorkflowApiClient(
        HttpClient http,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LmsWorkflowApiClient> logger)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<BaseResponse<LmsBarcodeResolutionClientDto>?> ResolveBarcodeAsync(
        string barcodeValue,
        CancellationToken cancellationToken = default)
    {
        var path = $"api/v1.0/workflow/integration/barcode/{Uri.EscapeDataString(barcodeValue)}";
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        ForwardContextHeaders(request);

        var response = await _http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("LMS barcode resolve failed {Status} for {Barcode}", response.StatusCode, barcodeValue);
            return BaseResponse<LmsBarcodeResolutionClientDto>.Fail($"LMS returned {(int)response.StatusCode}.");
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return await response.Content.ReadFromJsonAsync<BaseResponse<LmsBarcodeResolutionClientDto>>(
            jsonOptions,
            cancellationToken);
    }

    private void ForwardContextHeaders(HttpRequestMessage request)
    {
        var http = _httpContextAccessor.HttpContext;
        if (http is null)
            return;

        if (http.Request.Headers.TryGetValue("Authorization", out var auth))
            request.Headers.TryAddWithoutValidation("Authorization", auth.ToString());

        if (http.Request.Headers.TryGetValue("X-Tenant-Id", out var tenant))
            request.Headers.TryAddWithoutValidation("X-Tenant-Id", tenant.ToString());

        if (http.Request.Headers.TryGetValue("X-Facility-Id", out var facility))
            request.Headers.TryAddWithoutValidation("X-Facility-Id", facility.ToString());
    }
}
