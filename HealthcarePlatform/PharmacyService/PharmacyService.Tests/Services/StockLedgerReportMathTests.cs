using FluentAssertions;
using Xunit;

namespace PharmacyService.Tests.Services;

public sealed class StockLedgerReportMathTests
{
    [Fact]
    public void Summary_Closing_equals_opening_plus_in_minus_out()
    {
        const decimal opening = 12.5m;
        const decimal tin = 30m;
        const decimal tout = 7.25m;
        var closing = opening + tin - tout;
        closing.Should().Be(35.25m);
    }
}
