using Ardalis.GuardClauses;
using Buyz.Goodz.Application.DTOs;
using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Buyz.Goodz.Application.Queries;

public class ProductQueryService : IProductQueryService
{
    private readonly ILogger<ProductQueryService> _logger;
    private readonly IProductDtoMapper _mapper;
    private readonly IProductReadRepository _productReadRepository;

    public ProductQueryService(ILogger<ProductQueryService> logger,
        IProductDtoMapper mapper,
        IProductReadRepository productReadRepository)
    {
        _logger = Guard.Against.Null(logger);
        _mapper = Guard.Against.Null(mapper);
        _productReadRepository = Guard.Against.Null(productReadRepository);
    }

    public async IAsyncEnumerable<ProductDto> GetAllAsync()
    {
        _logger.LogDebug($"{nameof(GetAllAsync)}");
        var asyncEnumerable = _productReadRepository.GetAllAsync();
        await foreach (var product in asyncEnumerable)
        {
            yield return _mapper.MapToDto(product);
        }
    }

    public async IAsyncEnumerable<ProductDto> GetByColourAsync(string colour)
    {
        _logger.LogDebug($"GetByColourAsync({colour})");
        var domainColour = _mapper.MapToColourDomain(Guard.Against.NullOrWhiteSpace(colour));
        var asyncEnumerable = _productReadRepository.GetByColourAsync(domainColour);
        await foreach (var product in asyncEnumerable)
        {
            yield return _mapper.MapToDto(product);
        }
    }
}

