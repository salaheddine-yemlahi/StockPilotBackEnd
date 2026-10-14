
using System.Net;
using System.Net.Http.Json;
using StockPilot.Application.Products.DTOs;
using Xunit;

namespace StockPilot.Tests.Integration;

public class ProductsApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsOkAndPagedResult()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<PagedResultDto<ProductDto>>();

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.True(result.Page >= 1);
        Assert.True(result.PageSize >= 1);
        Assert.True(result.TotalCount >= 0);
    }
}
