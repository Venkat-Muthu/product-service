namespace Buyz.Goodz.Infrastructure.MongoDb.Documents;

public class ProductDocument : IEquatable<ProductDocument>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ColourDocument Colour { get; set; }
    public int StockLevel { get; set; }
    public DateTime CreatedInUtc { get; set; }
    public DateTime LastUpdatedInUtc { get; set; }

    public bool Equals(ProductDocument? other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) &&
               Name.Equals(other.Name) &&
               Price.Equals(other.Price) &&
               Colour.Equals(other.Colour);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ProductDocument);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id,
            Name,
            Price,
            Colour.GetHashCode,
            StockLevel);
    }
}
