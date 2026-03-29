namespace SharedService.Application.DTOs;

/// <summary>Basic Shared module descriptor.</summary>
public sealed class InfoResponseDto
{
    public string Service { get; set; } = null!;

    public string Version { get; set; } = "1.0";

    public string Module { get; set; } = "Shared";
}
