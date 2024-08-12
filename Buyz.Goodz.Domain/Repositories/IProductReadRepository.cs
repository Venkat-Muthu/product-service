using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.ValueObjects;
using System.Runtime.CompilerServices;

namespace Buyz.Goodz.Domain.Repositories;

public interface IProductReadRepository
{
    IAsyncEnumerable<Product> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);

    IAsyncEnumerable<Product> GetByColourAsync(Colour colour,
        [EnumeratorCancellation] CancellationToken cancellationToken = default);
}
