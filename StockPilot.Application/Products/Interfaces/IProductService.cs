
using StockPilot.Application.Products.DTOs;

namespace StockPilot.Application.Products.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductDto> CreateAsync(
        CreateProductDto dto,
        CancellationToken cancellationToken = default);
}
