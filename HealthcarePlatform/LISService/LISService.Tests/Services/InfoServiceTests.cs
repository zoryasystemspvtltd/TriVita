using FluentAssertions;
using LISService.Application.Services;
using Xunit;

namespace LISService.Tests.Services;

public sealed class InfoServiceTests
{
    [Fact]
    public void GetInfo_ReturnsLisModule()
    {
        var sut = new InfoService();

        var result = sut.GetInfo();

        result.Success.Should().BeTrue();
        result.Data!.Module.Should().Be("LIS");
        result.Data.Service.Should().Be("LISService");
    }
}
