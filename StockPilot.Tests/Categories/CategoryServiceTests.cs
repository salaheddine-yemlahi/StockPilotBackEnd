
using Moq;
using StockPilot.Application.Categories;
using StockPilot.Application.Categories.DTOs;
using StockPilot.Application.Categories.Interfaces;
using StockPilot.Domain.Entities;

namespace StockPilot.Tests.Categories;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _service = new CategoryService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Informatique" },
            new() { Id = 2, Name = "Mobilier" }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Informatique");
        Assert.Contains(result, c => c.Name == "Mobilier");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategory()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Informatique",
            Description = "Matériel informatique"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Informatique", result.Name);
        Assert.Equal("Matériel informatique", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidCategory_CreatesCategory()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            Name = "  Informatique  ",
            Description = "  Matériel informatique  "
        };

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync(
                "Informatique", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(
                It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal("Informatique", result.Name);
        Assert.Equal("Matériel informatique", result.Description);

        _repositoryMock.Verify(
            r => r.AddAsync(
                It.Is<Category>(c => c.Name == "Informatique"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenNameAlreadyExists_ThrowsException()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            Name = "Informatique"
        };

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync(
                "Informatique", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto));

        _repositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _service.UpdateAsync(
            99,
            new UpdateCategoryDto { Name = "Nouvelle catégorie" });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesCategory()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Informatique"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _repositoryMock
            .Setup(r => r.UpdateAsync(
                It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateAsync(
            1,
            new UpdateCategoryDto
            {
                Name = "  Informatique professionnelle  ",
                Description = "  Équipements  "
            });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Informatique professionnelle", result.Name);
        Assert.Equal("Équipements", result.Description);

        _repositoryMock.Verify(
            r => r.UpdateAsync(
                It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _service.DeleteAsync(99);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryHasProducts_ThrowsException()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Informatique",
            Products = new List<Product>
            {
                new() { Id = 10, Name = "Clavier" }
            }
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.DeleteAsync(1));

        _repositoryMock.Verify(
            r => r.DeleteAsync(
                It.IsAny<Category>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryIsEmpty_DeletesCategory()
    {
        // Arrange
        var category = new Category
        {
            Id = 1,
            Name = "Mobilier"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _repositoryMock
            .Setup(r => r.DeleteAsync(
                It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result);

        _repositoryMock.Verify(
            r => r.DeleteAsync(
                category, It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
