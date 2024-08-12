using AutoFixture;
using Buyz.Goodz.API.Controllers;
using Buyz.Goodz.Application.DTOs;
using Buyz.Goodz.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Buyz.Goodz.API.UnitTests;

public class ProductsControllerTests
{
    private readonly IFixture _fixture;
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductCommandService _productCommandService;
    private readonly IProductQueryService _productQueryService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _fixture = new Fixture();
        _logger = Substitute.For<ILogger<ProductsController>>();
        _productCommandService = Substitute.For<IProductCommandService>();
        _productQueryService = Substitute.For<IProductQueryService>();

        _controller = new ProductsController(
            _logger,
            _productCommandService,
            _productQueryService);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnAllProducts_WithStatus200()
    {
        // Arrange
        var productDtos = _fixture.CreateMany<ProductDto>().ToAsyncEnumerable();
        _productQueryService.GetAllAsync().Returns(productDtos);

        // Act
        var result = await (await _controller.GetProducts()).ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productDtos.CountAsync().Result, result.Count);
        _productQueryService.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task Get_ShouldReturnProductsByColour_WithStatus200()
    {
        // Arrange
        var colour = "Red";
        var productDtos = _fixture.CreateMany<ProductDto>().ToAsyncEnumerable();
        _productQueryService.GetByColourAsync(colour).Returns(productDtos);

        // Act
        var result = await (await _controller.Get(colour)).ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productDtos.CountAsync().Result, result.Count);
        _productQueryService.Received(1).GetByColourAsync(colour);
    }

    [Fact]
    public async Task Post_ShouldReturnCreatedStatus_WhenProductIsAddedSuccessfully()
    {
        // Arrange
        var createProductDto = _fixture.Create<CreateProductDto>();
        var productDto = _fixture.Create<ProductDto>();
        _productCommandService.AddAsync(createProductDto).Returns(Task.FromResult(productDto));

        // Act
        var result = await _controller.Post(createProductDto) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(nameof(_controller.GetProducts), result.ActionName);
        await _productCommandService.Received(1).AddAsync(createProductDto);
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var createProductDto = _fixture.Create<CreateProductDto>();
        _controller.ModelState.AddModelError("TestError", "Invalid model state");

        // Act
        var result = await _controller.Post(createProductDto) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        _logger.Received(1).LogWarning("Invalid product creation request");
        await _productCommandService.DidNotReceive().AddAsync(createProductDto);
    }

    [Fact]
    public async Task Post_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var createProductDto = _fixture.Create<CreateProductDto>();
        _productCommandService.AddAsync(createProductDto).Returns<ProductDto>(x => { throw new Exception("Error occurred"); });

        // Act
        var result = await _controller.Post(createProductDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Equal("Internal server error", result.Value);
        _logger.Received(1).LogError(Arg.Any<Exception>(), "Error occurred while creating a product");
    }
}
