namespace LISService.Application.Abstractions;

public interface ILisNotificationHelper
{
    Task NotifyLabReportReadyAsync(long labOrderId, long patientId, string? patientEmail, CancellationToken cancellationToken = default);
}
