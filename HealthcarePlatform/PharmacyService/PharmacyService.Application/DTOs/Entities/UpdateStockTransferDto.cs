namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdateStockTransferDto
{
    public string TransferNo { get; set; }
    public long FromFacilityId { get; set; }
    public long ToFacilityId { get; set; }
    public DateTime TransferOn { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? Notes { get; set; }
}