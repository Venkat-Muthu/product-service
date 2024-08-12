using Ardalis.GuardClauses;
using Buyz.Goodz.Application.DTOs;
using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Domain.Events;
using Buyz.Goodz.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Buyz.Goodz.Application.Commands;

public class ProductCommandService : IProductCommandService
{
    private readonly ILogger<ProductCommandService> _logger;
    private readonly IProductDtoMapper _mapper;
    private readonly IProductWriteRepository _productWriteRepository;
    private readonly IProductCreatedEventHandler _productCreatedEventHandler;

    public ProductCommandService(ILogger<ProductCommandService> logger,
        IProductDtoMapper mapper,
        IProductWriteRepository productWriteRepository,
        IProductCreatedEventHandler productCreatedEventHandler)
    {
        _logger = Guard.Against.Null(logger);
        _mapper = Guard.Against.Null(mapper);
        _productWriteRepository = Guard.Against.Null(productWriteRepository);
        _productCreatedEventHandler = Guard.Against.Null(productCreatedEventHandler);
    }
    public async Task<ProductDto> AddAsync(CreateProductDto createProductDto)
    {
        Guard.Against.Null(createProductDto);

        var product = _mapper.MapToDomain(createProductDto);
        await _productWriteRepository.AddAsync(product);

        foreach (var domainEvent in product.DomainEvents)
        {
            if (domainEvent is ProductCreatedEvent productCreatedEvent)
            {
                try
                {
                    _ = _productCreatedEventHandler.Handle(productCreatedEvent);
                    _logger.LogInformation("{Product} ({Id}) create event raised",
                        productCreatedEvent.Name,
                        productCreatedEvent.ProductId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "{Product} ({Id}) create event failed",
                        productCreatedEvent.Name,
                        productCreatedEvent.ProductId);
                }
            }
        }

        product.ClearDomainEvents();
        return _mapper.MapToDto(product);
    }
}
