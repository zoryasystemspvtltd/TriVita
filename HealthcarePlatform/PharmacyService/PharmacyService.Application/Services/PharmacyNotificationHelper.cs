using CommunicationService.Contracts.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PharmacyService.Application.Abstractions;
using PharmacyService.Application.Options;

namespace PharmacyService.Application.Services;

public sealed class PharmacyNotificationHelper : IPharmacyNotificationHelper
{
    private readonly INotificationApiClient _client;
    private readonly PharmacyNotificationIntegrationOptions _options;
    private readonly ILogger<PharmacyNotificationHelper> _logger;

    public PharmacyNotificationHelper(
        INotificationApiClient client,
        IOptions<PharmacyNotificationIntegrationOptions> options,
        ILogger<PharmacyNotificationHelper> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task NotifyPrescriptionCreatedAsync(long prescriptionId, long patientId, string? patientEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(patientEmail))
        {
            _logger.LogInformation("Skipping prescription notification (no email) for prescription {Id}.", prescriptionId);
            return;
        }

        if (_options.PatientRecipientTypeReferenceValueId == 0
            || _options.EmailChannelReferenceValueId == 0
            || _options.PriorityNormalReferenceValueId == 0)
        {
            _logger.LogWarning("PharmacyNotifications reference ids are not configured; skipping Communication call.");
            return;
        }

        var request = new CreateNotificationRequestDto
        {
            EventType = "Pharmacy.Prescription.Created",
            ReferenceId = prescriptionId,
            Context = new Dictionary<string, string>
            {
                ["patientId"] = patientId.ToString(),
                ["prescriptionId"] = prescriptionId.ToString()
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
                    TemplateCode = _options.PrescriptionCreatedTemplateCode
                }
            }
        };

        try
        {
            await _client.CreateNotificationAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue prescription notification for {PrescriptionId}.", prescriptionId);
        }
    }
}
