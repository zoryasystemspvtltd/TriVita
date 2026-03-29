namespace HMSService.Application.Abstractions;

/// <summary>Routes HMS domain events to CommunicationService (never sends SMTP/SMS directly).</summary>
public interface INotificationHelper
{
    Task NotifyAppointmentCreatedAsync(
        long appointmentId,
        long patientId,
        string? patientEmail,
        CancellationToken cancellationToken = default);
}
