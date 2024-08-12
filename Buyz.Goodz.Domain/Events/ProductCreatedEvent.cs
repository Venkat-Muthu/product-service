namespace Buyz.Goodz.Domain.Events;

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
        ProductId = productId;
        Name = name;
        Price = price;
        Colour = colour;
        StockLevel = stockLevel;
        CreatedInUtc = createdInUtc;
    }
}
