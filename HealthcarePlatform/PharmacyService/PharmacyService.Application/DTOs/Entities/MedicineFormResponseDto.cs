namespace PharmacyService.Application.DTOs.Entities;

public sealed class MedicineFormResponseDto
{
    public long Id { get; set; }
    public string FormCode { get; set; } = null!;
    public string FormName { get; set; } = null!;
    public string? Description { get; set; }
}
