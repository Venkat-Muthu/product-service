using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Buyz.Goodz.Application.DTOs;
using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Application.Queries;
using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.Repositories;
using Buyz.Goodz.Domain.ValueObjects;
using Buyz.Goodz.TestUtils.Mocks;
using Buyz.Goodz.TestUtils.SpecimenBuilders;
using NSubstitute;
using Xunit;

namespace Buyz.Goodz.Application.UnitTests.Queries;

public class ProductQueryServiceTests
{
    private readonly IFixture _fixture;
    private readonly MockLogger<ProductQueryService> _logger;
    private readonly IProductDtoMapper _mapper;
    private readonly IProductReadRepository _productReadRepository;
    private readonly ProductQueryService _sut;

    public ProductQueryServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Customizations.Add(new ProductArgCreatedInUtc(DateTime.UtcNow));
        _logger = Substitute.For<MockLogger<ProductQueryService>>();
        _mapper = Substitute.For<IProductDtoMapper>();
        _productReadRepository = Substitute.For<IProductReadRepository>();

        _sut = new ProductQueryService(
            _logger,
            _mapper,
            _productReadRepository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = _fixture.CreateMany<Product>();
        _productReadRepository.GetAllAsync().Returns(products.ToAsyncEnumerable());

        // Act
        var result = new List<ProductDto>();
        await foreach (var productDto in _sut.GetAllAsync())
        {
            result.Add(productDto);
        }

        // Assert
        Assert.NotEmpty(result);
        _productReadRepository.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetProductsByColourAsync_ShouldReturnFilteredProducts()
    {
        // Arrange
        var colour = "Red";
        var domainColour = new Colour("RedDomain");
        var products = _fixture.CreateMany<Product>();

        _mapper.MapToColourDomain(colour).Returns(domainColour);
        _productReadRepository.GetByColourAsync(domainColour).Returns(products.ToAsyncEnumerable());

        // Act
        var result = new List<ProductDto>();
        await foreach (var productDto in _sut.GetByColourAsync(colour))
        {
            result.Add(productDto);
        }

        // Assert
        Assert.NotEmpty(result);
        _productReadRepository.Received(1).GetByColourAsync(domainColour);
    }
}
