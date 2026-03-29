using FluentAssertions;
using LMSService.Application.Services;
using Xunit;

namespace LMSService.Tests.Services;

public sealed class InfoServiceTests
{
    [Fact]
    public void GetInfo_ReturnsLmsModule()
    {
        var sut = new InfoService();

        var result = sut.GetInfo();

        result.Success.Should().BeTrue();
        result.Data!.Module.Should().Be("LMS");
    }
}
