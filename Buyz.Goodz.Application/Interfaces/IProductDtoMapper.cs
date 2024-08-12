using Buyz.Goodz.Application.DTOs;
using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.ValueObjects;

namespace Buyz.Goodz.Application.Interfaces;

public interface IProductDtoMapper
{
    Product MapToDomain(CreateProductDto dto);
    ProductDto MapToDto(Product product);
    Colour MapToColourDomain(string colour);
}
