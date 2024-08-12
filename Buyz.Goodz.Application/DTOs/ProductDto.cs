namespace Buyz.Goodz.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Colour { get; set; }
    public int StockLevel { get; set; }
}