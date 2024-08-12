using Buyz.Goodz.Application.DTOs;
using Buyz.Goodz.Application.Interfaces;
using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.ValueObjects;

namespace Buyz.Goodz.Application.Mappers;

public class ProductDtoMapper : IProductDtoMapper
{
    public Product MapToDomain(CreateProductDto dto)
    {
        var colour = MapToColourDomain(dto.Colour);

        return Product.Create(dto.Id,
            dto.Name,
            dto.Price,
            colour,
            dto.StockLevel,
            DateTime.UtcNow);
    }

    public ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Colour = product.Colour.Name,
            StockLevel = product.StockLevel,
        };
    }

    public Colour MapToColourDomain(string colour)
    {
        return new Colour(colour);
    }
}