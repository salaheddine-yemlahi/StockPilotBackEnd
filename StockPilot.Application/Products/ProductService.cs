
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

    public async Task<PagedResultDto<ProductDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (products, totalCount) = await _repository.GetPagedAsync(
            page, pageSize, search, cancellationToken);

        return new PagedResultDto<ProductDto>
        {
            Items = products.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
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

    public async Task<ProductDto?> UpdateAsync(
        int id,
        UpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return null;

        var sku = dto.SKU.Trim();

        if (!string.Equals(
                product.SKU, sku, StringComparison.OrdinalIgnoreCase) &&
            await _repository.ExistsBySkuAsync(sku, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Un produit avec le SKU '{sku}' existe déjà.");
        }

        product.Name = dto.Name.Trim();
        product.SKU = sku;
        product.Description = dto.Description?.Trim();
        product.UnitPrice = dto.UnitPrice;
        product.QuantityInStock = dto.QuantityInStock;
        product.MinimumStockLevel = dto.MinimumStockLevel;

        await _repository.UpdateAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return false;

        await _repository.DeleteAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
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
