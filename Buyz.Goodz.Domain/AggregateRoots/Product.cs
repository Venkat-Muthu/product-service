using Ardalis.GuardClauses;
using Buyz.Goodz.Domain.Events;
using Buyz.Goodz.Domain.ValueObjects;

namespace Buyz.Goodz.Domain.AggregateRoots;

[Serializable]
public sealed class Product : IEquatable<Product>
{
    public static readonly Product Default = new Product(Guid.Empty,
        "Default",
        0.0M,
        Colour.Default,
        0,
        DateTime.MinValue,
        DateTime.MinValue);

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public Colour Colour { get; private set; }
    public int StockLevel { get; private set; }
    public DateTime CreatedInUtc { get; private set; }
    public DateTime LastUpdatedInUtc { get; private set; }

    private List<DomainEvent> _domainEvents = new List<DomainEvent>();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public static Product Create(Guid id,
        string name,
        decimal price,
        Colour colour,
        int stockLevel,
        DateTime createdInUtc)
    {
        _ = Guard.Against.Null(id);
        _ = Guard.Against.NullOrWhiteSpace(name);
        _ = Guard.Against.NegativeOrZero(price);
        _ = Guard.Against.Default(colour);
        _ = Guard.Against.Negative(stockLevel);
        _ = Guard.Against.OutOfRange(createdInUtc, nameof(createdInUtc), DateTime.MinValue, DateTime.UtcNow);

        var product = new Product(id,
            name,
            price,
            colour,
            stockLevel,
            createdInUtc,
            createdInUtc);

        product.AddDomainEvent(new ProductCreatedEvent(id,
            name,
            price,
            colour.Name,
            stockLevel,
            createdInUtc));

        return product;
    }

    public static Product Rehydrate(Guid id,
        string name,
        decimal price,
        Colour colour,
        int stockLevel,
        DateTime createdInUtc,
        DateTime lastUpdatedInUtc)
    {
        return new Product(id,
            name,
            price,
            colour,
            stockLevel,
            createdInUtc,
            lastUpdatedInUtc);
    }

    private Product(Guid id,
        string name,
        decimal price,
        Colour colour,
        int stockLevel,
        DateTime createdInUtc,
        DateTime lastUpdatedInUtc)
    {
        Id = id;
        Name = name;
        Price = price;
        Colour = colour;
        StockLevel = stockLevel;
        CreatedInUtc = createdInUtc;
        LastUpdatedInUtc = lastUpdatedInUtc;
    }

    private void AddDomainEvent(DomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void ChangePrice(decimal newPrice)
    {
        Price = Guard.Against.NegativeOrZero(newPrice);
        LastUpdatedInUtc = DateTime.UtcNow;
    }

    public bool Equals(Product? other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id) &&
               Name.Equals(other.Name) &&
               Price.Equals(other.Price) &&
               Colour.Equals(other.Colour);
    }

    // Override Equals and GetHashCode for value object comparison
    public override bool Equals(object obj)
    {
        return Equals(obj as Product);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id,
            Name,
            Price,
            Colour.GetHashCode,
            StockLevel);
    }

    public override string ToString()
    {
        return $"{Name} (ID: {Id}) - " +
            $"Price: {Price:C}, " +
            $"Stock: {StockLevel}, " +
            $"Colour: {Colour}";
    }
}
