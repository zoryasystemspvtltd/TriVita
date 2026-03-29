using CommunicationService.Application.Abstractions;

namespace CommunicationService.Application.Services;

public sealed class TemplateRenderer : ITemplateRenderer
{
    public string Render(string? template, IReadOnlyDictionary<string, string> data)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        var result = template;
        foreach (var kv in data)
        {
            var token = "{{" + kv.Key + "}}";
            result = result.Replace(token, kv.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }
}
