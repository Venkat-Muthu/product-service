using Buyz.Goodz.Application.DTOs;
using Buyz.Goodz.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buyz.Goodz.API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductCommandService _productCommandService;
    private readonly IProductQueryService _productQueryService;

    public ProductsController(ILogger<ProductsController> logger,
        IProductCommandService productCommandService,
        IProductQueryService productQueryService)
    {
        _logger = logger;
        _productCommandService = productCommandService;
        _productQueryService = productQueryService;
    }

    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <returns>An async stream of ProductDto</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    //[Authorize(Roles = "User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IAsyncEnumerable<ProductDto>> GetProducts()
    {
        _logger.LogInformation("Fetching all products");
        return await Task.FromResult(_productQueryService.GetAllAsync());
    }


    /// <summary>
    /// Gets products by colour.
    /// </summary>
    /// <param name="colour">The colour to filter products by</param>
    /// <returns>An async stream of ProductDto</returns>
    /// <response code="200">Returns the list of products with the specified colour</response>
    /// <response code="404">If no products with the specified colour are found</response>
    [HttpGet("{colour}")]
    //[Authorize(Roles = "User")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public async Task<IAsyncEnumerable<ProductDto>> Get(string colour)
    {
        _logger.LogInformation("Fetching products by colour: {Colour}", colour);
        return await Task.FromResult(_productQueryService.GetByColourAsync(colour));
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="createProductDto">The product to create</param>
    /// <returns>Action result</returns>
    /// <response code="201">Returns the newly created product</response>
    /// <response code="400">If the product creation request is invalid</response>
    /// <response code="500">If there is an internal server error</response>
    [HttpPost]
    //[Authorize(Roles = "PowerUser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Post([FromBody] CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid product creation request");
            return BadRequest(ModelState);
        }

        try
        {
            await _productCommandService.AddAsync(createProductDto);
            _logger.LogInformation("Product created successfully");
            return CreatedAtAction(nameof(GetProducts), new { id = createProductDto.Id }, createProductDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a product");
            return StatusCode(500, "Internal server error");
        }
    }
}
