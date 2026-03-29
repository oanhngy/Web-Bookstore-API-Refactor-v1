using BookstoreWeb.Application.Domain;
using BookstoreWeb.Application.Exceptions;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookstoreWeb.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _service = new ProductService(_mockProductRepo.Object, _mockLogger.Object);
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_WhenProductNotFound_ShouldThrowNotFoundException()
    {
        // Arrange: repo trả null
        _mockProductRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(99);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProductResponse()
    {
        // Arrange
        var product = new Product { ProductID = 1, Name = "Clean Code", Price = 200, ProductImages = new List<ProductImage>() };
        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal(1, result.ProductID);
        Assert.Equal("Clean Code", result.Name);
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_WhenProductNotFound_ShouldThrowNotFoundException()
    {
        // Arrange: product không tồn tại
        _mockProductRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        // Act
        var act = async () => await _service.DeleteAsync(99);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ShouldCallDeleteAsync()
    {
        // Arrange
        _mockProductRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

        // Act
        await _service.DeleteAsync(1);

        // Assert: DeleteAsync trên repo được gọi đúng 1 lần với đúng id
        _mockProductRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}
