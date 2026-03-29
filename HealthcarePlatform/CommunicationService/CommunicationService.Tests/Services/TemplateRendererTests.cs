using CommunicationService.Application.Services;
using FluentAssertions;
using Xunit;

namespace CommunicationService.Tests.Services;

public sealed class TemplateRendererTests
{
    private readonly TemplateRenderer _sut = new();

    [Fact]
    public void Render_ReplacesPlaceholders_CaseInsensitive()
    {
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = "Ada",
            ["Code"] = "X1"
        };

        var result = _sut.Render("Hello {{name}}, code {{CODE}}", data);

        result.Should().Be("Hello Ada, code X1");
    }

    [Fact]
    public void Render_WhenTemplateNull_ReturnsEmpty()
    {
        _sut.Render(null, new Dictionary<string, string>()).Should().BeEmpty();
    }
}
