
using Microsoft.EntityFrameworkCore;
using StockPilot.Application.StockMovements.Interfaces;
using StockPilot.Domain.Entities;
using StockPilot.Infrastructure.Persistence;

namespace StockPilot.Infrastructure.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly StockPilotDbContext _context;

    public StockMovementRepository(StockPilotDbContext context)
    {
        _context = context;
    }

    public async Task<List<StockMovement>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .OrderByDescending(m => m.Date)
            .ThenByDescending(m => m.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<StockMovement?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.StockMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .FirstOrDefaultAsync(
                m => m.Id == id,
                cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        // Le produit reste suivi par EF Core pour que sa quantité
        // puisse être modifiée et enregistrée.
        return await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == productId,
                cancellationToken);
    }

    public async Task AddAsync(
        StockMovement movement,
        CancellationToken cancellationToken = default)
    {
        await _context.StockMovements.AddAsync(
            movement,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
