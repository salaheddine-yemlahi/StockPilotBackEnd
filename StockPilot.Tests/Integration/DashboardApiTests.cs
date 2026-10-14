
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockPilot.Application.Dashboard.DTOs;
using StockPilot.Domain.Entities;
using StockPilot.Infrastructure.Persistence;

namespace StockPilot.Tests.Integration;

public class DashboardApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DashboardApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSummary_ReturnsCorrectStatistics()
    {
        // Arrange : créer les données de test
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<StockPilotDbContext>();

            var informatique = new Category
            {
                Name = $"Informatique-{Guid.NewGuid():N}"
            };

            var mobilier = new Category
            {
                Name = $"Mobilier-{Guid.NewGuid():N}"
            };

            db.Categories.AddRange(informatique, mobilier);

            db.Products.AddRange(
                new Product
                {
                    Name = "Produit normal",
                    SKU = $"TEST-{Guid.NewGuid():N}",
                    UnitPrice = 10m,
                    QuantityInStock = 10,
                    MinimumStockLevel = 3,
                    Category = informatique
                },
                new Product
                {
                    Name = "Produit stock faible",
                    SKU = $"TEST-{Guid.NewGuid():N}",
                    UnitPrice = 10m,
                    QuantityInStock = 2,
                    MinimumStockLevel = 3,
                    Category = informatique
                },
                new Product
                {
                    Name = "Produit en rupture",
                    SKU = $"TEST-{Guid.NewGuid():N}",
                    UnitPrice = 10m,
                    QuantityInStock = 0,
                    MinimumStockLevel = 3,
                    Category = mobilier
                });

            await db.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/Dashboard/summary");

        // Assert : vérifier la réponse HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var summary = await response.Content
            .ReadFromJsonAsync<DashboardDto>();

        Assert.NotNull(summary);
        Assert.Equal(3, summary.TotalProducts);
        Assert.Equal(2, summary.TotalCategories);
        Assert.Equal(120m, summary.TotalStockValue);
        Assert.Equal(1, summary.LowStockProducts);
        Assert.Equal(1, summary.OutOfStockProducts);
    }
}
