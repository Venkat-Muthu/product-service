using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Buyz.Goodz.Application.Commands;
using Buyz.Goodz.Application.DTOs;
using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.Events;
using Buyz.Goodz.Domain.Repositories;
using Buyz.Goodz.TestUtils.Mocks;
using Buyz.Goodz.TestUtils.SpecimenBuilders;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Buyz.Goodz.Application.UnitTests.Commands;

public class ProductCommandServiceTests
{
    private readonly IFixture _fixture;
    private readonly MockLogger<ProductCommandService> _logger;
    private readonly IProductDtoMapper _mapper;
    private readonly IProductWriteRepository _productWriteRepository;
    private readonly IProductCreatedEventHandler _productCreatedEventHandler;
    private readonly ProductCommandService _sut;

    public ProductCommandServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Customizations.Add(new ProductArgCreatedInUtc(DateTime.UtcNow));
        _logger = Substitute.For<MockLogger<ProductCommandService>>();
        _mapper = Substitute.For<IProductDtoMapper>();
        _productWriteRepository = Substitute.For<IProductWriteRepository>();
        _productCreatedEventHandler = Substitute.For<IProductCreatedEventHandler>();

        _sut = new ProductCommandService(
            _logger,
            _mapper,
            _productWriteRepository,
            _productCreatedEventHandler);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProductAndHandleEvents_WhenValidProductIsProvided()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        var createProductDto = _fixture.Build<CreateProductDto>()
            .With(dto => dto.Id, product.Id)
            .With(dto => dto.Name, product.Name)
            .Create();
        var productCreatedEvent = _fixture.Build<ProductCreatedEvent>()
            .FromFactory(() => new ProductCreatedEvent(product.Id,
            product.Name,
            product.Price,
            product.Colour.Name,
            product.StockLevel,
            product.CreatedInUtc))
        .Create();

        _mapper.MapToDomain(createProductDto).Returns(product);
        _mapper.MapToDto(product).Returns(_fixture.Create<ProductDto>());

        // Act
        var result = await _sut.AddAsync(createProductDto);

        // Assert
        await _productWriteRepository.Received(1).AddAsync(product);
        await _productCreatedEventHandler.Received(1).Handle(Arg.Any<ProductCreatedEvent>());

        var message = $"{productCreatedEvent.Name} ({productCreatedEvent.ProductId}) create event raised";
        _logger.Received(1).Log(LogLevel.Information,
            Arg.Is<string>(s => s.Equals(message)));
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldLogError_WhenEventHandlingFails()
    {
        // Arrange
        var product = _fixture.Create<Product>();
        var createProductDto = _fixture.Build<CreateProductDto>()
            .With(dto => dto.Id, product.Id)
            .With(dto => dto.Name, product.Name)
            .Create();
        var productCreatedEvent = _fixture.Build<ProductCreatedEvent>()
            .FromFactory(() => new ProductCreatedEvent(product.Id,
            product.Name,
            product.Price,
            product.Colour.Name,
            product.StockLevel,
            product.CreatedInUtc))
            .Create();
        var productDto = _fixture.Build<ProductDto>()
            .FromFactory(() => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Colour = product.Colour.Name,
                StockLevel = product.StockLevel
            })
            .Create();

        _mapper.MapToDomain(createProductDto).Returns(product);
        _mapper.MapToDto(product).Returns(productDto);
        _productCreatedEventHandler.Handle(Arg.Any<ProductCreatedEvent>())
                                   .Returns(x => throw new Exception("Event Handling Failed"));

        // Act
        var result = await _sut.AddAsync(createProductDto);

        // Assert
        await _productWriteRepository.Received(1).AddAsync(product);
        var message = $"{productCreatedEvent.Name} ({productCreatedEvent.ProductId}) create event failed";
        _logger.Received(1).Log(LogLevel.Error, Arg.Is<string>(s => s.Equals(message)));

        result.Should().NotBeNull();
    }
}
