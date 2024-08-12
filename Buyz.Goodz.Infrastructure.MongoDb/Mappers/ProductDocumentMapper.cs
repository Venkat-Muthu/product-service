using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.ValueObjects;
using Buyz.Goodz.Infrastructure.MongoDb.Documents;

namespace Buyz.Goodz.Infrastructure.MongoDb.Mappers;

public class ProductDocumentMapper : IProductDocumentMapper
{
    public ProductDocument MapToDocument(Product product)
    {
        return new ProductDocument
        {
            Id = product.Id,
            Name = product.Name,
            Colour = MapToDocument(product.Colour),
            Price = product.Price,
            StockLevel = product.StockLevel,
            CreatedInUtc = product.CreatedInUtc,
            LastUpdatedInUtc = product.LastUpdatedInUtc
        };
    }

    public Product MapToDomain(ProductDocument document)
    {
        return Product.Rehydrate(document.Id,
            document.Name,
            document.Price,
            MapToDomain(document.Colour),
            document.StockLevel,
            document.CreatedInUtc,
            document.LastUpdatedInUtc);
    }

    public ColourDocument MapToDocument(Colour colour)
    {
        return new ColourDocument
        {
            Name = colour.Name,
        };
    }

    public Colour MapToDomain(ColourDocument document)
    {
        return new Colour(document.Name);
    }
}