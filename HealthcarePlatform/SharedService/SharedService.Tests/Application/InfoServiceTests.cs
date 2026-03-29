using FluentAssertions;
using SharedService.Application.Services;
using Xunit;

namespace SharedService.Tests.Application;

public sealed class InfoServiceTests
{
    [Fact]
    public void GetInfo_Should_Return_SharedService_Payload()
    {
        var sut = new InfoService();

        var result = sut.GetInfo();

        result.Success.Should().BeTrue();
        result.Data!.Service.Should().Be("SharedService");
        result.Data.Version.Should().Be("1.0");
        result.Data.Module.Should().Be("Shared");
    }
}
