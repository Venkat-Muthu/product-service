using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.ValueObjects;
using Buyz.Goodz.Infrastructure.MongoDb.Documents;
using Buyz.Goodz.Infrastructure.MongoDb.Mappers;
using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;
using Buyz.Goodz.TestUtils.Mocks;
using Buyz.Goodz.TestUtils.SpecimenBuilders;
using Xunit;

namespace Buyz.Goodz.Infrastructure.MongoDb.UnitTests;

public class ProductRepositoryTests
{
    private readonly IFixture _fixture;
    private readonly MockLogger<ProductRepository> _logger;
    private readonly IMongoCollection<ProductDocument> _productCollection;
    private readonly IMongoDbContext _mockContext;
    private readonly IProductDocumentMapper _mapper;
    private readonly ProductRepository _sut;

    public ProductRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Customizations.Add(new ProductArgCreatedInUtc(DateTime.UtcNow));
        _logger = Substitute.For<MockLogger<ProductRepository>>();
        _productCollection = Substitute.For<IMongoCollection<ProductDocument>>();
        _mockContext = Substitute.For<IMongoDbContext>();
        _mapper = Substitute.For<IProductDocumentMapper>();
        _mockContext.GetCollection<ProductDocument>("Products").Returns(_productCollection);

        _sut = new ProductRepository(_logger,
            _mockContext,
            _mapper);
    }

    [Fact]
    public async Task AddAsync_ShouldInsertProduct()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        var productDocument = _fixture.Create<ProductDocument>();
        _mapper.MapToDocument(product).Returns(productDocument);
        CancellationToken cancellationToken = default;

        // Act
        await _sut.AddAsync(product, cancellationToken);

        // Assert
        await _productCollection.Received(1).InsertOneAsync(Arg.Is<ProductDocument>(p => p.Equals(productDocument)), null, default);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var productDocuments = _fixture.CreateMany<ProductDocument>(3).ToList();
        var products = _fixture.CreateMany<Product>(3).ToList();
        _mapper.MapToDomain(Arg.Any<ProductDocument>()).Returns(products[0], products[1], products[2]);
        var mockCursor = CreateAsyncCursor(productDocuments);
        _productCollection.FindAsync(Arg.Any<FilterDefinition<ProductDocument>>(), Arg.Any<FindOptions<ProductDocument>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockCursor));

        // Act
        var result = await _sut.GetAllAsync().ToListAsync();

        // Assert
        productDocuments.Count.Should().Be(result.Count);
        _mapper.Received(3).MapToDomain(Arg.Any<ProductDocument>());
        result.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetByColourAsync_ShouldReturnProductsByColour()
    {
        // Arrange
        var colour = new Colour("Red");
        var productDocuments = _fixture.Build<ProductDocument>()
            .With(doc => doc.Colour, new ColourDocument { Name = colour.Name })
        .CreateMany(3).ToList();

        var products = _fixture.CreateMany<Product>(3).ToList();
        _mapper.MapToDomain(Arg.Any<ProductDocument>())
            .Returns(products[0], products[1], products[2]);

        var mockCursor = CreateAsyncCursor(productDocuments);
        _productCollection.FindAsync(Arg.Any<FilterDefinition<ProductDocument>>(), Arg.Any<FindOptions<ProductDocument>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(mockCursor));

        // Act
        var result = await _sut.GetByColourAsync(colour).ToListAsync();

        // Assert
        _mapper.Received(3).MapToDomain(Arg.Any<ProductDocument>());
        productDocuments.Should().HaveCount(result.Count);
        result.Should().BeEquivalentTo(products);
    }

    // Helper method to create a mock cursor for async enumerables
    private IAsyncCursor<T> CreateAsyncCursor<T>(IEnumerable<T> items)
    {
        var mockCursor = Substitute.For<IAsyncCursor<T>>();
        mockCursor.Current.Returns(items);

        mockCursor.MoveNext(Arg.Any<CancellationToken>()).Returns(true, false);
        mockCursor.MoveNextAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true), Task.FromResult(false));

        return mockCursor;
    }
}
