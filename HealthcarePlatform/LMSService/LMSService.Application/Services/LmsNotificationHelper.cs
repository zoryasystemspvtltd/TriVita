using CommunicationService.Contracts.Notifications;
using LMSService.Application.Abstractions;
using LMSService.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LMSService.Application.Services;

public sealed class LmsNotificationHelper : ILmsNotificationHelper
{
    private readonly INotificationApiClient _client;
    private readonly LmsNotificationIntegrationOptions _options;
    private readonly ILogger<LmsNotificationHelper> _logger;

    public LmsNotificationHelper(
        INotificationApiClient client,
        IOptions<LmsNotificationIntegrationOptions> options,
        ILogger<LmsNotificationHelper> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task NotifyCourseAssignedAsync(long enrollmentId, long studentId, string? studentEmail, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(studentEmail))
        {
            _logger.LogInformation("Skipping LMS notification (no email) for enrollment {Id}.", enrollmentId);
            return;
        }

        if (_options.StudentRecipientTypeReferenceValueId == 0
            || _options.EmailChannelReferenceValueId == 0
            || _options.PriorityNormalReferenceValueId == 0)
        {
            _logger.LogWarning("LmsNotifications reference ids are not configured; skipping Communication call.");
            return;
        }

        var request = new CreateNotificationRequestDto
        {
            EventType = "LMS.Course.Assigned",
            ReferenceId = enrollmentId,
            Context = new Dictionary<string, string>
            {
                ["studentId"] = studentId.ToString(),
                ["enrollmentId"] = enrollmentId.ToString()
            },
            PriorityReferenceValueId = _options.PriorityNormalReferenceValueId,
            Recipients = new[]
            {
                new RecipientInputDto
                {
                    RecipientTypeReferenceValueId = _options.StudentRecipientTypeReferenceValueId,
                    RecipientId = studentId,
                    Email = studentEmail.Trim(),
                    IsPrimary = true
                }
            },
            Channels = new[]
            {
                new ChannelRequestDto
                {
                    ChannelTypeReferenceValueId = _options.EmailChannelReferenceValueId,
                    TemplateCode = _options.CourseAssignedTemplateCode
                }
            }
        };

        try
        {
            await _client.CreateNotificationAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue LMS notification for enrollment {EnrollmentId}.", enrollmentId);
        }
    }
}
