
using StockPilot.Domain.Entities;

namespace StockPilot.Application.StockMovements.Interfaces;

public interface IStockMovementRepository
{
    Task<List<StockMovement>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<StockMovement?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Product?> GetProductByIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        StockMovement movement,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
