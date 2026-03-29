namespace HMSService.Application.DTOs.Extended;

public sealed class UpdatePaymentModeDto
{
    public string ModeCode { get; set; }
    public string ModeName { get; set; }
    public int SortOrder { get; set; }
}