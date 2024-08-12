using System.Net.Http.Json;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Buyz.Goodz.Application.Interfaces;
using NSubstitute;
using Microsoft.AspNetCore.Authentication;
using Buyz.Goodz.Application.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Buyz.Goodz.Tests.Integration;

public class ProductsControllerIntegrationTests : IClassFixture<WebApplicationFactory<API.Program>>
{
    private readonly HttpClient _client;
    private readonly IProductCommandService _productCommandService;
    private readonly IProductQueryService _productQueryService;

    public ProductsControllerIntegrationTests(WebApplicationFactory<API.Program> factory)
    {
        _productCommandService = Substitute.For<IProductCommandService>();
        _productQueryService = Substitute.For<IProductQueryService>();

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Add the authentication handler for testing
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

                // Replace real services with substitutes
                services.RemoveAll(typeof(IProductCommandService));
                services.RemoveAll(typeof(IProductQueryService));
                services.AddSingleton(_productCommandService);
                services.AddSingleton(_productQueryService);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var productDtos = new[]
        {
                new ProductDto { Id = Guid.NewGuid(), Name = "Product1", Colour = "Red" },
                new ProductDto { Id = Guid.NewGuid(), Name = "Product2", Colour = "Blue" }
            };

        _productQueryService.GetAllAsync().Returns(productDtos.ToAsyncEnumerable());

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var returnedProducts = await response.Content.ReadFromJsonAsync<ProductDto[]>();
        Assert.NotNull(returnedProducts);
        Assert.Equal(2, returnedProducts.Length);
    }

    [Fact]
    public async Task Get_ShouldReturnProductsByColour()
    {
        // Arrange
        var colour = "Red";
        var productDtos = new[]
        {
                new ProductDto { Id = Guid.NewGuid(), Name = "Product1", Colour = "Red" }
            };

        _productQueryService.GetByColourAsync(colour).Returns(productDtos.ToAsyncEnumerable());

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");
        var response = await _client.GetAsync($"/api/products/{colour}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var returnedProducts = await response.Content.ReadFromJsonAsync<ProductDto[]>();
        Assert.NotNull(returnedProducts);
        Assert.Single(returnedProducts);
        Assert.Equal(colour, returnedProducts[0].Colour);
    }

    [Fact]
    public async Task Post_ShouldCreateProduct()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Id = Guid.NewGuid(),
            Name = "NewProduct",
            Price = 10.50M,
            Colour = "Green",
            StockLevel = 10
        };
        var productDto = new ProductDto
        {
            Id = createProductDto.Id,
            Name = createProductDto.Name,
            Price = createProductDto.Price,
            Colour = createProductDto.Colour,
            StockLevel = createProductDto.StockLevel
        };

        _productCommandService.AddAsync(createProductDto).Returns(Task.FromResult(productDto));

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");
        var response = await _client.PostAsJsonAsync("/api/products", createProductDto);

        // If the test fails, log or inspect the response content
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"BadRequest: {content}");
        }

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        await _productCommandService.Received(1).AddAsync(Arg.Is<CreateProductDto>(dto => dto.Name == "NewProduct"));
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenModelIsInvalid()
    {
        // Arrange
        var createProductDto = new CreateProductDto(); // Invalid model with missing required properties

        // Act
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");
        var response = await _client.PostAsJsonAsync("/api/products", createProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await _productCommandService.DidNotReceive().AddAsync(Arg.Any<CreateProductDto>());
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check which method is being tested and assign roles accordingly
        string role = Context.Request.Method == HttpMethod.Post.Method ? "PowerUser" : "User";

        var claims = new[] { new Claim(ClaimTypes.Name, "TestUser"), new Claim(ClaimTypes.Role, role) };
        var identity = new ClaimsIdentity(claims, "TestScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
