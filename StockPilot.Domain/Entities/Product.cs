
namespace StockPilot.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }

    public int QuantityInStock { get; set; }

    public int MinimumStockLevel { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Category relationship
    public int? CategoryId { get; set; }

    public Category? Category { get; set; }
}
