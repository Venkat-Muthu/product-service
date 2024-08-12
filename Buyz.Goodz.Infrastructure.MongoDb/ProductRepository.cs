using Ardalis.GuardClauses;
using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.Repositories;
using Buyz.Goodz.Domain.ValueObjects;
using Buyz.Goodz.Infrastructure.MongoDb.Documents;
using Buyz.Goodz.Infrastructure.MongoDb.Mappers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace Buyz.Goodz.Infrastructure.MongoDb;

public class ProductRepository : IProductWriteRepository, IProductReadRepository
{
    private readonly ILogger<ProductRepository> _logger;
    private readonly IMongoCollection<ProductDocument> _products;
    private readonly IProductDocumentMapper _productDocumentMapper;

    public ProductRepository(ILogger<ProductRepository> logger,
        IMongoDbContext context,
        IProductDocumentMapper productDocumentMapper)
    {
        _logger = Guard.Against.Null(logger);
        Guard.Against.Null(context);
        _products = context.GetCollection<ProductDocument>("Products");
        _productDocumentMapper = productDocumentMapper;
    }

    public async Task AddAsync(Product product,
        CancellationToken cancellationToken = default)
    {
        var productDocument = _productDocumentMapper.MapToDocument(product);
        await _products.InsertOneAsync(productDocument, null, cancellationToken);
        _logger.LogInformation("{ProductName} with ({Id}) added successfully",
            productDocument.Name,
            productDocument.Id);
    }

    public async IAsyncEnumerable<Product> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var filter = Builders<ProductDocument>.Filter.Empty;
        using var cursor = await _products.FindAsync(filter, cancellationToken: cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var productDocument in cursor.Current)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return _productDocumentMapper.MapToDomain(productDocument);
            }
        }
    }

    public async IAsyncEnumerable<Product> GetByColourAsync(Colour colour,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var filter = Builders<ProductDocument>.Filter.Eq(p => p.Colour.Name, colour.Name);
        using var cursor = await _products.FindAsync(filter, cancellationToken: cancellationToken);

        while (await cursor.MoveNextAsync(cancellationToken))
        {
            foreach (var productDocument in cursor.Current)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return _productDocumentMapper.MapToDomain(productDocument);
            }
        }
    }
}