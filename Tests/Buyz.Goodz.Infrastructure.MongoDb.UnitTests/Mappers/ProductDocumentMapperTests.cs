using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.ValueObjects;
using Buyz.Goodz.Infrastructure.MongoDb.Documents;
using Buyz.Goodz.Infrastructure.MongoDb.Mappers;
using FluentAssertions;
using Buyz.Goodz.TestUtils.SpecimenBuilders;
using Xunit;

namespace Buyz.Goodz.Infrastructure.MongoDb.UnitTests.Mappers;

public class ProductDocumentMapperTests
{
    private readonly IFixture _fixture;
    private readonly IProductDocumentMapper _mapper;

    public ProductDocumentMapperTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Customizations.Add(new ProductArgCreatedInUtc(DateTime.UtcNow));
        _mapper = new ProductDocumentMapper();
    }

    [Theory, AutoData]
    public void MapToDocument_ShouldMapProductToProductDocument()
    {
        var product = _fixture.Create<Product>();

        // Act
        var result = _mapper.MapToDocument(product);

        // Assert
        product.Id.Should().Be(result.Id);
        product.Name.Should().Be(result.Name);
        product.Price.Should().Be(result.Price);
        product.StockLevel.Should().Be(result.StockLevel);
        product.CreatedInUtc.Should().Be(result.CreatedInUtc);
        product.LastUpdatedInUtc.Should().Be(result.LastUpdatedInUtc);
        product.Colour.Name.Should().Be(result.Colour.Name);
    }

    [Theory, AutoData]
    public void MapToDomain_ShouldMapProductDocumentToProduct(ProductDocument document)
    {
        // Act
        var result = _mapper.MapToDomain(document);

        // Assert
        document.Id.Should().Be(result.Id);
        document.Name.Should().Be(result.Name);
        document.Price.Should().Be(result.Price);
        document.StockLevel.Should().Be(result.StockLevel);
        document.CreatedInUtc.Should().Be(result.CreatedInUtc);
        document.LastUpdatedInUtc.Should().Be(result.LastUpdatedInUtc);
        document.Colour.Name.Should().Be(result.Colour.Name);
    }

    [Theory, AutoData]
    public void MapToDocument_ShouldMapColourToColourDocument(Colour colour)
    {
        // Act
        var result = _mapper.MapToDocument(colour);

        // Assert
        colour.Name.Should().Be(result.Name);
    }

    [Theory, AutoData]
    public void MapToDomain_ShouldMapColourDocumentToColour(ColourDocument document)
    {
        // Act
        var result = _mapper.MapToDomain(document);

        // Assert
        result.Name.Should().Be(document.Name);
    }
}