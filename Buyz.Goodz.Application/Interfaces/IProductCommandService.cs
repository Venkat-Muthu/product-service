using Buyz.Goodz.Application.DTOs;

namespace Buyz.Goodz.Application.Interfaces;

public interface IProductCommandService
{
    Task<ProductDto> AddAsync(CreateProductDto createProductDto);
}
