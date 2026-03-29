namespace HMSService.Application.Integration;

public sealed class LmsIntegrationOptions
{
    public const string SectionName = "Lms";

    public string BaseUrl { get; set; } = "http://localhost:5151/";
}
