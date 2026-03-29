using CommunicationService.Contracts.Notifications;
using LISService.Application.Abstractions;
using LISService.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LISService.Application.Services;

public sealed class LisNotificationHelper : ILisNotificationHelper
{
    private readonly INotificationApiClient _client;
    private readonly LisNotificationIntegrationOptions _options;
    private readonly ILogger<LisNotificationHelper> _logger;

    public LisNotificationHelper(
        INotificationApiClient client,
        IOptions<LisNotificationIntegrationOptions> options,
        ILogger<LisNotificationHelper> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task NotifyLabReportReadyAsync(long labOrderId, long patientId, string? patientEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(patientEmail))
        {
            _logger.LogInformation("Skipping lab report notification (no email) for order {Id}.", labOrderId);
            return;
        }

        if (_options.PatientRecipientTypeReferenceValueId == 0
            || _options.EmailChannelReferenceValueId == 0
            || _options.PriorityNormalReferenceValueId == 0)
        {
            _logger.LogWarning("LisNotifications reference ids are not configured; skipping Communication call.");
            return;
        }

        var request = new CreateNotificationRequestDto
        {
            EventType = "LIS.LabReport.Ready",
            ReferenceId = labOrderId,
            Context = new Dictionary<string, string>
            {
                ["patientId"] = patientId.ToString(),
                ["labOrderId"] = labOrderId.ToString()
            },
            PriorityReferenceValueId = _options.PriorityNormalReferenceValueId,
            Recipients = new[]
            {
                new RecipientInputDto
                {
                    RecipientTypeReferenceValueId = _options.PatientRecipientTypeReferenceValueId,
                    RecipientId = patientId,
                    Email = patientEmail.Trim(),
                    IsPrimary = true
                }
            },
            Channels = new[]
            {
                new ChannelRequestDto
                {
                    ChannelTypeReferenceValueId = _options.EmailChannelReferenceValueId,
                    TemplateCode = _options.LabReportReadyTemplateCode
                }
            }
        };

        try
        {
            await _client.CreateNotificationAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue lab report notification for order {LabOrderId}.", labOrderId);
        }
    }
}
