namespace Healthcare.Common.Integration.SharedService;

/// <summary>Configuration for calling SharedService (enterprise / facility validation).</summary>
public sealed class SharedServiceClientOptions
{
    public const string SectionName = "SharedService";

    /// <summary>Base URL of SharedService (e.g. https://host/shared/ or http://localhost:5700/).</summary>
    public string BaseUrl { get; set; } = "http://localhost:5153/";

    /// <summary>When false, <see cref="IFacilityTenantValidator"/> returns a stub context without HTTP (local dev / tests).</summary>
    public bool ValidateFacilityWithSharedService { get; set; } = true;

    public int TimeoutSeconds { get; set; } = 30;
}
