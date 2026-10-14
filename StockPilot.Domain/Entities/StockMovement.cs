
namespace StockPilot.Domain.Entities;

public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public StockMovementType Type { get; set; }

    public int Quantity { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public string? Reason { get; set; }
}
