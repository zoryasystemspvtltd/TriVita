namespace LISService.Application.Abstractions;

public interface ILisNotificationHelper
{
    Task NotifyLabReportReadyAsync(long labOrderId, long patientId, string? patientEmail, CancellationToken cancellationToken = default);

    Task NotifyAnalyzerResultReadyAsync(
        long lmsTestBookingItemId,
        long patientId,
        string? patientEmail,
        CancellationToken cancellationToken = default);
}
