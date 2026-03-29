using BookstoreWeb.Application.Domain;
using BookstoreWeb.Application.Exceptions;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Application.Services;
using BookstoreWeb.Application.DTOs.Orders;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookstoreWeb.Tests.Services;

public class OrderServiceTests
{
    //mock dependencies
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockLogger = new Mock<ILogger<OrderService>>();
        _service = new OrderService(_mockOrderRepo.Object, _mockLogger.Object);
    }

    //GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange: repo trả null
        _mockOrderRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Order?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(99);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ShouldReturnOrderResponse()
    {
        // Arrange
        var order = new Order { OrderID = 1, Status = "Confirmed", UserID = "user1" };
        _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal(1, result.OrderId);
        Assert.Equal("Confirmed", result.Status);
    }

    //CancelAsync
    [Fact]
    public async Task CancelAsync_WhenOrderNotFound_ShouldThrowNotFoundException()
    {
        // Arrange: order null
        _mockOrderRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Order?)null);

        // Act
        var act = async () => await _service.CancelAsync(99, "user1");

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task CancelAsync_WhenOrderBelongsToDifferentUser_ShouldThrowNotFoundException()
    {
        // Arrange: order tồn tại nhưng sai userId (security check)
        var order = new Order { OrderID = 1, Status = "Confirmed", UserID = "other-user" };
        _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        // Act
        var act = async () => await _service.CancelAsync(1, "user1");

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task CancelAsync_WhenStatusIsNew_ShouldThrowValidationException()
    {
        // Arrange: status "New" không nằm trong cancellableStatuses
        var order = new Order { OrderID = 1, Status = "New", UserID = "user1" };
        _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        // Act
        var act = async () => await _service.CancelAsync(1, "user1");

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task CancelAsync_WhenStatusIsCheckedOut_ShouldUpdateStatusToCancelled()
    {
        // Arrange
        var order = new Order { OrderID = 1, Status = "Checked Out", UserID = "user1" };
        _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        // Act
        await _service.CancelAsync(1, "user1");

        // Assert: verify UpdateAsync được gọi đúng 1 lần với status = "Cancelled"
        _mockOrderRepo.Verify(
            r => r.UpdateAsync(It.Is<Order>(o => o.Status == "Cancelled")),
            Times.Once);
    }

    //ConfirmAsync

    [Fact]
    public async Task ConfirmAsync_WhenStatusIsNotCheckedOut_ShouldThrowValidationException()
    {
        // Arrange: order đang ở status "New", không thể confirm
        var order = new Order { OrderID = 1, Status = "New", UserID = "user1" };
        _mockOrderRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        var request = new ConfirmOrderRequest
        {
            FullName = "Test User",
            Email = "test@test.com",
            Phone = "0123456789",
            Address = "123 Main St",
            PaymentMethod = "COD"
        };

        // Act
        var act = async () => await _service.ConfirmAsync(1, request, "user1");

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }
}
