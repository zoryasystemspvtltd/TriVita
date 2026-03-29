namespace CommunicationService.Contracts.Notifications;

public sealed class RecipientInputDto
{
    public long RecipientTypeReferenceValueId { get; set; }

    public long? RecipientId { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsPrimary { get; set; }
}
