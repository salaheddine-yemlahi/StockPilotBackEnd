
using System.ComponentModel.DataAnnotations;
using StockPilot.Domain.Entities;

namespace StockPilot.Application.StockMovements.DTOs;

public class CreateStockMovementDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [EnumDataType(typeof(StockMovementType))]
    public StockMovementType Type { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }
}
