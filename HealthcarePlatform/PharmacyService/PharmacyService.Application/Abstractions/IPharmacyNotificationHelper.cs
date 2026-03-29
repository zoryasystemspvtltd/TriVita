namespace PharmacyService.Application.Abstractions;

public interface IPharmacyNotificationHelper
{
    Task NotifyPrescriptionCreatedAsync(long prescriptionId, long patientId, string? patientEmail, CancellationToken cancellationToken = default);
}
