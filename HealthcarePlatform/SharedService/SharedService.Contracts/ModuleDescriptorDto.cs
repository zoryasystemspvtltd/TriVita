namespace SharedService.Contracts;

public sealed class ModuleDescriptorDto
{
    public string ModuleCode { get; set; } = "SHARED";

    public string DisplayName { get; set; } = "Shared domain";
}
