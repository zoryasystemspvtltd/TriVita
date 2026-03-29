namespace HMSService.Application.DTOs.Extended;

public sealed class PaymentModeResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string ModeCode { get; set; }
    public string ModeName { get; set; }
    public int SortOrder { get; set; }
}