namespace LMSService.Application.Abstractions;

public interface ILmsNotificationHelper
{
    Task NotifyCourseAssignedAsync(long enrollmentId, long studentId, string? studentEmail, CancellationToken cancellationToken = default);
}
