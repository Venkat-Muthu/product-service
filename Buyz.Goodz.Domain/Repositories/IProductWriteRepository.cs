using Buyz.Goodz.Domain.AggregateRoots;

namespace Buyz.Goodz.Domain.Repositories;

public interface IProductWriteRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
}