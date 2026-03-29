using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisSampleBarcode : BaseEntity
{
    public long SampleCollectionId { get; set; }
    public string BarcodeValue { get; set; } = null!;
    public string? QrPayload { get; set; }
    public long? IdentifierTypeReferenceValueId { get; set; }
    public DateTime? PrintedOn { get; set; }
}
