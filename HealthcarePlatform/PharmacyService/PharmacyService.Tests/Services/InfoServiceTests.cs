using FluentAssertions;
using PharmacyService.Application.Services;
using Xunit;

namespace PharmacyService.Tests.Services;

public sealed class InfoServiceTests
{
    [Fact]
    public void GetInfo_ReturnsPharmacyModule()
    {
        var sut = new InfoService();

        var result = sut.GetInfo();

        result.Success.Should().BeTrue();
        result.Data!.Module.Should().Be("Pharmacy");
    }
}
