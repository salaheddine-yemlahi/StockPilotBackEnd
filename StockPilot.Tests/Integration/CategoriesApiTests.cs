
using System.Net;
using System.Net.Http.Json;
using StockPilot.Application.Categories.DTOs;
using StockPilot.Tests.Integration;

namespace StockPilot.Tests.Integration;

public class CategoriesApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/api/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ReturnsCreated()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            Name = "Categorie Integration Test",
            Description = "Test de creation"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Categories", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetCategory_WhenNotFound_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/Categories/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
