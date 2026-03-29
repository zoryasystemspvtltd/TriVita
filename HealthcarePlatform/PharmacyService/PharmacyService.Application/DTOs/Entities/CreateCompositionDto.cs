namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateCompositionDto
{
    public string CompositionName { get; set; }
    public string? CompositionCode { get; set; }
    public string? Notes { get; set; }
}