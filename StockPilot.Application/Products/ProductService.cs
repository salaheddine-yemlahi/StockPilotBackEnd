
using StockPilot.Application.Products.DTOs;
using StockPilot.Application.Products.Interfaces;
using StockPilot.Domain.Entities;

namespace StockPilot.Application.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _repository.GetAllAsync(cancellationToken);

        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);

        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var sku = dto.SKU.Trim();

        if (await _repository.ExistsBySkuAsync(sku, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Un produit avec le SKU '{sku}' existe déjà.");
        }

        var product = new Product
        {
            Name = dto.Name.Trim(),
            SKU = sku,
            Description = dto.Description?.Trim(),
            UnitPrice = dto.UnitPrice,
            QuantityInStock = dto.QuantityInStock,
            MinimumStockLevel = dto.MinimumStockLevel
        };

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            QuantityInStock = product.QuantityInStock,
            MinimumStockLevel = product.MinimumStockLevel
        };
    }
}
