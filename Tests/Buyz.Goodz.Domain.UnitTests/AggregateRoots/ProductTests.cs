using AutoFixture.Idioms;
using AutoFixture;
using Buyz.Goodz.Domain.AggregateRoots;
using Xunit;
using FluentAssertions;
using Buyz.Goodz.TestUtils.SpecimenBuilders;

namespace Buyz.Goodz.Domain.UnitTests.AggregateRoots;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenGuardClauseFails()
    {
        var fixture = new Fixture();

        var assertion = new GuardClauseAssertion(fixture,
            new CompositeBehaviorExpectation(
                new NullReferenceBehaviorExpectation(),
                new EmptyStringBehaviorExpectation(),
                new WhiteSpaceStringBehaviorExpectation()));
        assertion.Verify(typeof(Product).GetConstructors());
    }

    [Theory]
    [InlineData(-1.1)]
    [InlineData(0.0)]
    public void ChangePrice_ShouldThrowException_WhenPriceIsZeroOrNegative(decimal newPrice)
    {
        var fixture = new Fixture();
        fixture.Customizations.Add(new ProductArgCreatedInUtc(DateTime.UtcNow));

        var product = fixture.Create<Product>();

        Action action = () => product.ChangePrice(newPrice);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Required input newPrice cannot be zero or negative. (Parameter 'newPrice')");
    }

    [Fact]
    public void Product_Should_BeEqual_When_PropertiesAreSame()
    {
        var fixture = new Fixture();
        fixture.Customizations.Add(new ProductArgCreatedInUtc(DateTime.UtcNow));
        var product1 = fixture.Create<Product>();

        var product2 = Product.Rehydrate(product1.Id,
            product1.Name,
            product1.Price,
            product1.Colour,
            product1.StockLevel,
            product1.CreatedInUtc,
            product1.LastUpdatedInUtc);

        product1.Should().Be(product2);
    }

    [Fact]
    public void Product_Should_NotBeEqual_When_AnyPropertyIsDifferent()
    {
        var fixture = new Fixture();
        fixture.Customizations.Add(new ProductArgCreatedInUtc(DateTime.UtcNow));
        var product1 = fixture.Create<Product>();
        var product2 = fixture.Create<Product>();

        product1.Should().NotBe(product2);
    }
}
