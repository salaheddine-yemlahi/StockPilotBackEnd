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
        var dto = new CreateProductDto
        {
            Name = "Clavier Logitech",
            SKU = "CLAV-001",
            Description = "Clavier sans fil",
            UnitPrice = 49.99m,
            QuantityInStock = 20,
            MinimumStockLevel = 5
        };

        var result = await _service.CreateAsync(dto);

        Assert.Equal(1, result.Id);
        Assert.Equal("Clavier Logitech", result.Name);
        Assert.Equal("CLAV-001", result.SKU);
        Assert.Equal(49.99m, result.UnitPrice);
        Assert.Single(_repository.Products);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSku_ThrowsInvalidOperationException()
    {
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

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(duplicateDto));

        Assert.Single(_repository.Products);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingProduct_UpdatesProduct()
    {
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

        var result = await _service.UpdateAsync(created.Id, updateDto);

        Assert.NotNull(result);
        Assert.Equal("Clavier Logitech Pro", result.Name);
        Assert.Equal(59.99m, result.UnitPrice);
        Assert.Equal(15, result.QuantityInStock);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ReturnsNull()
    {
        var dto = new UpdateProductDto
        {
            Name = "Produit",
            SKU = "SKU-001",
            UnitPrice = 10m,
            QuantityInStock = 5,
            MinimumStockLevel = 1
        };

        var result = await _service.UpdateAsync(999, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingProduct_DeletesProduct()
    {
        var created = await _service.CreateAsync(new CreateProductDto
        {
            Name = "Souris",
            SKU = "SOURIS-001",
            UnitPrice = 25m,
            QuantityInStock = 10,
            MinimumStockLevel = 2
        });

        var result = await _service.DeleteAsync(created.Id);

        Assert.True(result);
        Assert.Empty(_repository.Products);
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ReturnsFalse()
    {
        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageAndSearchResults()
    {
        await _service.CreateAsync(new CreateProductDto
        {
            Name = "Clavier Logitech",
            SKU = "CLAV-001",
            UnitPrice = 49.99m,
            QuantityInStock = 20,
            MinimumStockLevel = 5
        });

        await _service.CreateAsync(new CreateProductDto
        {
            Name = "Souris Logitech",
            SKU = "SOURIS-001",
            UnitPrice = 25m,
            QuantityInStock = 10,
            MinimumStockLevel = 2
        });

        await _service.CreateAsync(new CreateProductDto
        {
            Name = "Écran Dell",
            SKU = "ECRAN-001",
            UnitPrice = 199m,
            QuantityInStock = 8,
            MinimumStockLevel = 2
        });

        var result = await _service.GetPagedAsync(
            page: 1,
            pageSize: 1,
            search: "Logitech");

        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Single(result.Items);
        Assert.Contains("Logitech", result.Items[0].Name);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Products { get; } = [];

        public Task<List<Product>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var products = Products
                .OrderBy(p => p.Name)
                .ToList();

            return Task.FromResult(products);
        }

        public Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? search,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Product> query = Products;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();

                query = query.Where(p =>
                    p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    p.SKU.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null &&
                     p.Description.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            var filteredProducts = query
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            var totalCount = filteredProducts.Count;

            var items = filteredProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult((items, totalCount));
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
            var exists = Products.Any(p =>
                string.Equals(p.SKU, sku, StringComparison.OrdinalIgnoreCase));

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