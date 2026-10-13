
using StockPilot.Application.Products;
using StockPilot.Application.Products.DTOs;
using StockPilot.Application.Products.Interfaces;
using StockPilot.Domain.Entities;

namespace StockPilot.Tests;

public class ProductServiceTests
{
    private readonly FakeProductRepository _repository;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repository = new FakeProductRepository();
        _service = new ProductService(_repository);
    }

    [Fact]
    public async Task CreateAsync_WithValidProduct_ReturnsCreatedProduct()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Clavier Logitech",
            SKU = "CLAV-001",
            Description = "Clavier sans fil",
            UnitPrice = 49.99m,
            QuantityInStock = 20,
            MinimumStockLevel = 5
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Clavier Logitech", result.Name);
        Assert.Equal("CLAV-001", result.SKU);
        Assert.Equal(49.99m, result.UnitPrice);
        Assert.Single(_repository.Products);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSku_ThrowsInvalidOperationException()
    {
        // Arrange
        await _service.CreateAsync(new CreateProductDto
        {
            Name = "Premier produit",
            SKU = "SKU-001",
            UnitPrice = 10m,
            QuantityInStock = 5,
            MinimumStockLevel = 1
        });

        var duplicateDto = new CreateProductDto
        {
            Name = "Deuxième produit",
            SKU = "SKU-001",
            UnitPrice = 20m,
            QuantityInStock = 10,
            MinimumStockLevel = 2
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(duplicateDto));

        Assert.Single(_repository.Products);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingProduct_UpdatesProduct()
    {
        // Arrange
        var created = await _service.CreateAsync(new CreateProductDto
        {
            Name = "Clavier",
            SKU = "CLAV-001",
            UnitPrice = 49.99m,
            QuantityInStock = 20,
            MinimumStockLevel = 5
        });

        var updateDto = new UpdateProductDto
        {
            Name = "Clavier Logitech Pro",
            SKU = "CLAV-001",
            Description = "Version professionnelle",
            UnitPrice = 59.99m,
            QuantityInStock = 15,
            MinimumStockLevel = 3
        };

        // Act
        var result = await _service.UpdateAsync(created.Id, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Clavier Logitech Pro", result.Name);
        Assert.Equal(59.99m, result.UnitPrice);
        Assert.Equal(15, result.QuantityInStock);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ReturnsNull()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Name = "Produit",
            SKU = "SKU-001",
            UnitPrice = 10m,
            QuantityInStock = 5,
            MinimumStockLevel = 1
        };

        // Act
        var result = await _service.UpdateAsync(999, dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingProduct_DeletesProduct()
    {
        // Arrange
        var created = await _service.CreateAsync(new CreateProductDto
        {
            Name = "Souris",
            SKU = "SOURIS-001",
            UnitPrice = 25m,
            QuantityInStock = 10,
            MinimumStockLevel = 2
        });

        // Act
        var result = await _service.DeleteAsync(created.Id);

        // Assert
        Assert.True(result);
        Assert.Empty(_repository.Products);
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ReturnsFalse()
    {
        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Products { get; } = [];

        public Task<List<Product>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                Products.OrderBy(p => p.Name).ToList());
        }

        public Task<Product?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var product = Products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<bool> ExistsBySkuAsync(
            string sku,
            CancellationToken cancellationToken = default)
        {
            var exists = Products.Any(
                p => string.Equals(
                    p.SKU,
                    sku,
                    StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(exists);
        }

        public Task AddAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            Products.Add(product);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(
            Product product,
            CancellationToken cancellationToken = default)
        {
            Products.Remove(product);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var nextId = Products.Count == 0
                ? 1
                : Products.Max(p => p.Id) + 1;

            foreach (var product in Products.Where(p => p.Id == 0))
            {
                product.Id = nextId++;
            }

            return Task.CompletedTask;
        }
    }
}
