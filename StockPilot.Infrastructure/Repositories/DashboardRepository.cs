
using Microsoft.EntityFrameworkCore;
using StockPilot.Application.Dashboard.DTOs;
using StockPilot.Application.Dashboard.Interfaces;
using StockPilot.Infrastructure.Persistence;

namespace StockPilot.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly StockPilotDbContext _context;

    public DashboardRepository(StockPilotDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var totalProducts = await _context.Products
            .CountAsync(cancellationToken);

        var totalCategories = await _context.Categories
            .CountAsync(cancellationToken);

        var totalStockValue = await _context.Products
            .SumAsync(
                p => (decimal)p.UnitPrice * p.QuantityInStock,
                cancellationToken);

        var lowStockProducts = await _context.Products
            .CountAsync(
                p => p.QuantityInStock > 0 &&
                     p.QuantityInStock <= p.MinimumStockLevel,
                cancellationToken);

        var outOfStockProducts = await _context.Products
            .CountAsync(
                p => p.QuantityInStock == 0,
                cancellationToken);

        return new DashboardDto
        {
            TotalProducts = totalProducts,
            TotalCategories = totalCategories,
            TotalStockValue = totalStockValue,
            LowStockProducts = lowStockProducts,
            OutOfStockProducts = outOfStockProducts
        };
    }
}
