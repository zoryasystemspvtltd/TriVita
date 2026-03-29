using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisReportDeliveryOtp : BaseEntity
{
    public long ReportHeaderId { get; set; }
    public string OtpHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public DateTime? ConsumedOn { get; set; }
    public long? DeliveryChannelReferenceValueId { get; set; }
}
