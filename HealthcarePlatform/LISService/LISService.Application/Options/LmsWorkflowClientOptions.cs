namespace LISService.Application.Options;

public sealed class LmsWorkflowClientOptions
{
    public const string SectionName = "LmsWorkflow";

    /// <summary>Base URL of LMSService (e.g. https://host:port/).</summary>
    public string BaseUrl { get; set; } = "http://localhost:5700/";
}
