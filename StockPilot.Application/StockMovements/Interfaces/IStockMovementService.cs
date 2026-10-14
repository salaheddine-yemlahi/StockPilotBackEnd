
using StockPilot.Application.StockMovements.DTOs;

namespace StockPilot.Application.StockMovements.Interfaces;

public interface IStockMovementService
{
    Task<List<StockMovementDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<StockMovementDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<StockMovementDto> CreateAsync(
        CreateStockMovementDto dto,
        CancellationToken cancellationToken = default);
}
