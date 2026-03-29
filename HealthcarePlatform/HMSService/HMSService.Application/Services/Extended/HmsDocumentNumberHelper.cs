namespace HMSService.Application.Services.Extended;

/// <summary>Generates human-readable document numbers for HMS headers (prescriptions, bills, visits).</summary>
internal static class HmsDocumentNumberHelper
{
    public static string Generate(string prefix) =>
        $"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
}
