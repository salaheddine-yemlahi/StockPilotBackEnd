
using StockPilot.Application.StockMovements.DTOs;
using StockPilot.Application.StockMovements.Interfaces;
using StockPilot.Domain.Entities;

namespace StockPilot.Application.StockMovements;

public class StockMovementService : IStockMovementService
{
    private readonly IStockMovementRepository _repository;

    public StockMovementService(IStockMovementRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<StockMovementDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var movements = await _repository.GetAllAsync(cancellationToken);

        return movements.Select(MapToDto).ToList();
    }

    public async Task<StockMovementDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var movement = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        return movement is null ? null : MapToDto(movement);
    }

    public async Task<StockMovementDto> CreateAsync(
        CreateStockMovementDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Quantity <= 0)
        {
            throw new ArgumentException(
                "La quantité doit être strictement positive.");
        }

        if (!Enum.IsDefined(typeof(StockMovementType), dto.Type))
        {
            throw new ArgumentException(
                "Le type de mouvement est invalide.");
        }

        var product = await _repository.GetProductByIdAsync(
            dto.ProductId,
            cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException(
                $"Le produit {dto.ProductId} est introuvable.");
        }

        if (dto.Type == StockMovementType.Entry)
        {
            if (dto.Quantity > int.MaxValue - product.QuantityInStock)
            {
                throw new InvalidOperationException(
                    "La quantité totale dépasserait la limite autorisée.");
            }

            product.QuantityInStock += dto.Quantity;
        }
        else
        {
            if (dto.Quantity > product.QuantityInStock)
            {
                throw new InvalidOperationException(
                    $"Stock insuffisant. Quantité disponible : " +
                    $"{product.QuantityInStock}.");
            }

            product.QuantityInStock -= dto.Quantity;
        }

        var movement = new StockMovement
        {
            ProductId = product.Id,
            Type = dto.Type,
            Quantity = dto.Quantity,
            Date = DateTime.UtcNow,
            Reason = string.IsNullOrWhiteSpace(dto.Reason)
                ? null
                : dto.Reason.Trim()
        };

        await _repository.AddAsync(movement, cancellationToken);

        // Le produit modifié et le mouvement sont enregistrés
        // ensemble par le même DbContext et le même SaveChanges.
        await _repository.SaveChangesAsync(cancellationToken);

        movement.Product = product;

        return MapToDto(movement);
    }

    private static StockMovementDto MapToDto(StockMovement movement)
    {
        return new StockMovementDto
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            ProductName = movement.Product.Name,
            SKU = movement.Product.SKU,
            Type = movement.Type,
            Quantity = movement.Quantity,
            Date = movement.Date,
            Reason = movement.Reason
        };
    }
}
