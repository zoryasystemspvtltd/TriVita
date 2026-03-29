using System.Text.Json;
using CommunicationService.Contracts.Notifications;
using HMSService.Application.Abstractions;
using HMSService.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMSService.Application.Services;

public sealed class NotificationHelper : INotificationHelper
{
    private readonly INotificationApiClient _client;
    private readonly HmsNotificationIntegrationOptions _options;
    private readonly ILogger<NotificationHelper> _logger;

    public NotificationHelper(
        INotificationApiClient client,
        IOptions<HmsNotificationIntegrationOptions> options,
        ILogger<NotificationHelper> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task NotifyAppointmentCreatedAsync(
        long appointmentId,
        long patientId,
        string? patientEmail,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(patientEmail))
        {
            _logger.LogInformation("Skipping appointment notification (no patient email) for appointment {Id}.", appointmentId);
            return;
        }

        if (_options.PatientRecipientTypeReferenceValueId == 0
            || _options.EmailChannelReferenceValueId == 0
            || _options.PriorityNormalReferenceValueId == 0)
        {
            _logger.LogWarning("HmsNotifications reference ids are not configured; skipping Communication call.");
            return;
        }

        var request = new CreateNotificationRequestDto
        {
            EventType = "HMS.Appointment.Created",
            ReferenceId = appointmentId,
            Context = new Dictionary<string, string>
            {
                ["patientId"] = patientId.ToString(),
                ["appointmentId"] = appointmentId.ToString()
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
                    TemplateCode = _options.AppointmentCreatedTemplateCode
                }
            }
        };

        try
        {
            var result = await _client.CreateNotificationAsync(request, cancellationToken);
            if (result is null)
                _logger.LogWarning("CommunicationService returned empty body for appointment {Id}.", appointmentId);
            else
                _logger.LogInformation("Queued notification {NotificationId} for appointment {AppointmentId}.", result.Id, appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue appointment notification for {AppointmentId}.", appointmentId);
        }
    }
}
