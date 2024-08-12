using Buyz.Goodz.Application.DTOs;

namespace Buyz.Goodz.Application.Interfaces;

public interface IProductQueryService
{
    IAsyncEnumerable<ProductDto> GetAllAsync();

    IAsyncEnumerable<ProductDto> GetByColourAsync(string colour);
}
