
namespace StockPilot.Application.Dashboard.DTOs;

public class DashboardDto
{
    public int TotalProducts { get; set; }

    public int TotalCategories { get; set; }

    public decimal TotalStockValue { get; set; }

    public int LowStockProducts { get; set; }

    public int OutOfStockProducts { get; set; }
}
