using Ardalis.GuardClauses;
using Buyz.Goodz.Domain.Events;

namespace Buyz.Goodz.Domain.UnitTests.Events;

public sealed class ProductCreatedEvent : DomainEvent
{
    public Guid ProductId { get; }
    public string Name { get; }
    public decimal Price { get; }
    public string Colour { get; }
    public int StockLevel { get; }
    public DateTime CreatedInUtc { get; }

    public ProductCreatedEvent(Guid productId,
        string name,
        decimal price,
        string colour,
        int stockLevel,
        DateTime createdInUtc)
    {
        ProductId = Guard.Against.Null(productId);
        Name = Guard.Against.NullOrWhiteSpace(name);
        Price = Guard.Against.NegativeOrZero(price);
        Colour = Guard.Against.NullOrWhiteSpace(colour);
        StockLevel = Guard.Against.NegativeOrZero(stockLevel);
        CreatedInUtc = createdInUtc;
    }
}
