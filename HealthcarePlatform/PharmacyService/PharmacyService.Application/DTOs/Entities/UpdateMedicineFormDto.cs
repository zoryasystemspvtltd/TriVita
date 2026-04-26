namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdateMedicineFormDto
{
    public string FormCode { get; set; } = null!;
    public string FormName { get; set; } = null!;
    public string? Description { get; set; }
}
