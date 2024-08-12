using AutoFixture;
using AutoFixture.Idioms;
using Buyz.Goodz.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Buyz.Goodz.Domain.UnitTests.ValueObjects;

public class ColourTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenGuardClauseFails()
    {
        // Arrange
        var fixture = new Fixture();

        var assertion = new GuardClauseAssertion(fixture,
            new CompositeBehaviorExpectation(
            new NullReferenceBehaviorExpectation(),
            new EmptyStringBehaviorExpectation(),
            new WhiteSpaceStringBehaviorExpectation()));

        // Assert
        assertion.Verify(typeof(Colour).GetConstructors());
    }

    [Fact]
    public void Color_Should_BeEqual_When_NamesAreSame()
    {
        var colour1 = new Colour("Red");
        var colour2 = new Colour("Red");

        colour1.Should().Be(colour2);
    }

    [Fact]
    public void Color_Should_NotBeEqual_When_NamesAreDifferent()
    {
        var colourRed = new Colour("Red");
        var colourBlue = new Colour("Blue");

        colourRed.Should().NotBe(colourBlue);
    }
}