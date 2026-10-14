
using Moq;
using StockPilot.Application.StockMovements;
using StockPilot.Application.StockMovements.DTOs;
using StockPilot.Application.StockMovements.Interfaces;
using StockPilot.Domain.Entities;

namespace StockPilot.Tests.StockMovements;

public class StockMovementServiceTests
{
    private readonly Mock<IStockMovementRepository> _repositoryMock;
    private readonly StockMovementService _service;

    public StockMovementServiceTests()
    {
        _repositoryMock = new Mock<IStockMovementRepository>();
        _service = new StockMovementService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllMovements()
    {
        // Arrange
        var movements = new List<StockMovement>
        {
            CreateMovement(1, StockMovementType.Entry, 10),
            CreateMovement(2, StockMovementType.Exit, 3)
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(movements);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(StockMovementType.Entry, result[0].Type);
        Assert.Equal(StockMovementType.Exit, result[1].Type);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMovementExists_ReturnsMovement()
    {
        // Arrange
        var movement = CreateMovement(1, StockMovementType.Entry, 10);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movement);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Clavier", result.ProductName);
        Assert.Equal("CLAV-001", result.SKU);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMovementDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockMovement?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_Entry_IncreasesProductStock()
    {
        // Arrange
        var product = CreateProduct(quantity: 10);

        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(
                product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repositoryMock
            .Setup(r => r.AddAsync(
                It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dto = new CreateStockMovementDto
        {
            ProductId = product.Id,
            Type = StockMovementType.Entry,
            Quantity = 5,
            Reason = "  Réapprovisionnement  "
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(15, product.QuantityInStock);
        Assert.Equal(StockMovementType.Entry, result.Type);
        Assert.Equal(5, result.Quantity);
        Assert.Equal("Réapprovisionnement", result.Reason);
        Assert.Equal("Clavier", result.ProductName);

        _repositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Exit_DecreasesProductStock()
    {
        // Arrange
        var product = CreateProduct(quantity: 10);

        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(
                product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _repositoryMock
            .Setup(r => r.AddAsync(
                It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var dto = new CreateStockMovementDto
        {
            ProductId = product.Id,
            Type = StockMovementType.Exit,
            Quantity = 4
        };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(6, product.QuantityInStock);
        Assert.Equal(StockMovementType.Exit, result.Type);
        Assert.Equal(4, result.Quantity);
    }

    [Fact]
    public async Task CreateAsync_WhenQuantityIsZero_ThrowsArgumentException()
    {
        var dto = new CreateStockMovementDto
        {
            ProductId = 1,
            Type = StockMovementType.Entry,
            Quantity = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));

        _repositoryMock.Verify(
            r => r.GetProductByIdAsync(
                It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenQuantityIsNegative_ThrowsArgumentException()
    {
        var dto = new CreateStockMovementDto
        {
            ProductId = 1,
            Type = StockMovementType.Entry,
            Quantity = -2
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WhenTypeIsInvalid_ThrowsArgumentException()
    {
        var dto = new CreateStockMovementDto
        {
            ProductId = 1,
            Type = (StockMovementType)99,
            Quantity = 2
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));

        _repositoryMock.Verify(
            r => r.GetProductByIdAsync(
                It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenProductDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(
                999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var dto = new CreateStockMovementDto
        {
            ProductId = 999,
            Type = StockMovementType.Entry,
            Quantity = 2
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_WhenStockIsInsufficient_ThrowsInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct(quantity: 3);

        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(
                product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var dto = new CreateStockMovementDto
        {
            ProductId = product.Id,
            Type = StockMovementType.Exit,
            Quantity = 5
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto));

        Assert.Equal(3, product.QuantityInStock);

        _repositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _repositoryMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenEntryExceedsMaximumStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var product = CreateProduct(quantity: int.MaxValue);

        _repositoryMock
            .Setup(r => r.GetProductByIdAsync(
                product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var dto = new CreateStockMovementDto
        {
            ProductId = product.Id,
            Type = StockMovementType.Entry,
            Quantity = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto));

        Assert.Equal(int.MaxValue, product.QuantityInStock);

        _repositoryMock.Verify(
            r => r.AddAsync(
                It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Product CreateProduct(int quantity)
    {
        return new Product
        {
            Id = 1,
            Name = "Clavier",
            SKU = "CLAV-001",
            UnitPrice = 25.99m,
            QuantityInStock = quantity,
            MinimumStockLevel = 5
        };
    }

    private static StockMovement CreateMovement(
        int id,
        StockMovementType type,
        int quantity)
    {
        var product = CreateProduct(quantity: 10);

        return new StockMovement
        {
            Id = id,
            ProductId = product.Id,
            Product = product,
            Type = type,
            Quantity = quantity,
            Date = DateTime.UtcNow
        };
    }
}
