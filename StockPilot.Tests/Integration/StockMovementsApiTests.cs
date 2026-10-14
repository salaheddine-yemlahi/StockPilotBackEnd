
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using StockPilot.Application.StockMovements.DTOs;
using StockPilot.Domain.Entities;
using StockPilot.Infrastructure.Persistence;

namespace StockPilot.Tests.Integration;

public class StockMovementsApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public StockMovementsApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/api/StockMovements");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenMovementDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/StockMovements/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithValidEntry_ReturnsCreatedAndUpdatesStock()
    {
        // Arrange
        var productId = await CreateProductAsync(10);

        var dto = new CreateStockMovementDto
        {
            ProductId = productId,
            Type = StockMovementType.Entry,
            Quantity = 5,
            Reason = "Test integration"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/StockMovements", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var movement = await response.Content
            .ReadFromJsonAsync<StockMovementDto>();

        Assert.NotNull(movement);
        Assert.Equal(productId, movement.ProductId);
        Assert.Equal(StockMovementType.Entry, movement.Type);
        Assert.Equal(5, movement.Quantity);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StockPilotDbContext>();
        var product = await db.Products.FindAsync(productId);

        Assert.NotNull(product);
        Assert.Equal(15, product.QuantityInStock);
    }

    [Fact]
    public async Task Create_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var dto = new CreateStockMovementDto
        {
            ProductId = 999999,
            Type = StockMovementType.Entry,
            Quantity = 1
        };

        var response = await _client.PostAsJsonAsync(
            "/api/StockMovements", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenQuantityIsZero_ReturnsBadRequest()
    {
        var productId = await CreateProductAsync(10);

        var dto = new CreateStockMovementDto
        {
            ProductId = productId,
            Type = StockMovementType.Entry,
            Quantity = 0
        };

        var response = await _client.PostAsJsonAsync(
            "/api/StockMovements", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenStockIsInsufficient_ReturnsConflict()
    {
        var productId = await CreateProductAsync(2);

        var dto = new CreateStockMovementDto
        {
            ProductId = productId,
            Type = StockMovementType.Exit,
            Quantity = 5
        };

        var response = await _client.PostAsJsonAsync(
            "/api/StockMovements", dto);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private async Task<int> CreateProductAsync(int quantity)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StockPilotDbContext>();

        var product = new Product
        {
            Name = $"Produit test {Guid.NewGuid():N}",
            SKU = $"TEST-{Guid.NewGuid():N}",
            Description = "Produit pour test d'intégration",
            UnitPrice = 10m,
            QuantityInStock = quantity,
            MinimumStockLevel = 1
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        return product.Id;
    }
}
