using FluentAssertions;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.Tests.Domain.Shared;

public class TleParserTests
{
    // Real ISS TLE lines
    private const string IssLine1 = "1 25544U 98067A   21275.52069444  .00001520  00000-0  35029-4 0  9993";
    private const string IssLine2 = "2 25544  51.6442 337.6640 0001772  35.5820 324.5240 15.50377579303371";

    [Fact]
    public void Parse_IssLines_InclinationIsCorrect()
    {
        var oe = TleParser.Parse(IssLine1, IssLine2);
        oe.Inclination.Should().BeApproximately(51.64, 0.01);
    }

    [Fact]
    public void Parse_NullLine1_ThrowsInvalidOrbitalElementsException()
    {
        var act = () => TleParser.Parse(null!, IssLine2);
        act.Should().Throw<InvalidOrbitalElementsException>();
    }

    [Fact]
    public void Parse_ShortLine_ThrowsInvalidOrbitalElementsException()
    {
        var act = () => TleParser.Parse(IssLine1, "short");
        act.Should().Throw<InvalidOrbitalElementsException>();
    }
}
