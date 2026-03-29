using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsPaymentMode : BaseEntity
{
    public string ModeCode { get; set; } = null!;
    public string ModeName { get; set; } = null!;
    public int SortOrder { get; set; }
}