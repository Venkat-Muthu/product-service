using Buyz.Goodz.Domain.AggregateRoots;
using Buyz.Goodz.Domain.ValueObjects;
using Buyz.Goodz.Infrastructure.MongoDb.Documents;

namespace Buyz.Goodz.Infrastructure.MongoDb.Mappers;

public interface IProductDocumentMapper
{
    ColourDocument MapToDocument(Colour colour);
    ProductDocument MapToDocument(Product product);
    Colour MapToDomain(ColourDocument document);
    Product MapToDomain(ProductDocument document);
}
