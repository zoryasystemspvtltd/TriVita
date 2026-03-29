namespace LISService.Application.DTOs;

/// <summary>Basic LIS module descriptor for Swagger schemas.</summary>
public sealed class InfoResponseDto
{
    public string Service { get; set; } = null!;

    public string Version { get; set; } = "1.0";

    public string Module { get; set; } = "LIS";
}
