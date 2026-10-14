
using StockPilot.Domain.Entities;

namespace StockPilot.Application.StockMovements.DTOs;

public class StockMovementDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public StockMovementType Type { get; set; }

    public int Quantity { get; set; }

    public DateTime Date { get; set; }

    public string? Reason { get; set; }
}
