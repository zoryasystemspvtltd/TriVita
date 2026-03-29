namespace LISService.Contracts;

/// <summary>Inter-service DTO for capability discovery.</summary>
public sealed class ModuleDescriptorDto
{
    public string ModuleCode { get; set; } = "LIS";

    public string DisplayName { get; set; } = "Laboratory Information System";
}
