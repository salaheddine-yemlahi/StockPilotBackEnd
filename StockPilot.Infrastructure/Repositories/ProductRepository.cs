
using Microsoft.EntityFrameworkCore;
using StockPilot.Application.Products.Interfaces;
using StockPilot.Domain.Entities;
using StockPilot.Infrastructure.Persistence;

namespace StockPilot.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StockPilotDbContext _context;

    public ProductRepository(StockPilotDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AnyAsync(p => p.SKU == sku, cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
