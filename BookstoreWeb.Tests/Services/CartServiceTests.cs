using BookstoreWeb.Application.Domain;
using BookstoreWeb.Application.Exceptions;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Application.Services;
using BookstoreWeb.Application.DTOs.Cart;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookstoreWeb.Tests.Services;

public class CartServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ILogger<CartService>> _mockLogger;
    private readonly CartService _service;

    public CartServiceTests()
    {
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<CartService>>();
        _service = new CartService(_mockOrderRepo.Object, _mockProductRepo.Object, _mockLogger.Object);
    }

    // --- AddToCartAsync ---

    [Fact]
    public async Task AddToCartAsync_WhenProductNotFound_ShouldThrowNotFoundException()
    {
        // Arrange: product không tồn tại
        _mockProductRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        var request = new AddToCartRequest { ProductId = 99, Quantity = 1 };

        // Act
        var act = async () => await _service.AddToCartAsync("user1", request);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task AddToCartAsync_WhenCartIsNull_ShouldCreateNewCartAndCallAddAsync()
    {
        // Arrange: product tồn tại, user chưa có cart
        var product = new Product { ProductID = 1, Name = "Book A", Price = 100 };
        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockOrderRepo.Setup(r => r.GetCartByUserIdAsync("user1")).ReturnsAsync((Order?)null);

        var request = new AddToCartRequest { ProductId = 1, Quantity = 2 };

        // Act
        await _service.AddToCartAsync("user1", request);

        // Assert: AddAsync được gọi 1 lần (tạo cart mới)
        _mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task AddToCartAsync_WhenItemAlreadyInCart_ShouldIncreaseQuantity()
    {
        // Arrange: cart đã có item productId=1 với quantity=2
        var product = new Product { ProductID = 1, Price = 100 };
        var existingItem = new OrderDetail { ProductID = 1, Quantity = 2, UnitPrice = 100 };
        var cart = new Order
        {
            OrderID = 10,
            UserID = "user1",
            Status = "New",
            OrderDetails = new List<OrderDetail> { existingItem }
        };

        _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mockOrderRepo.Setup(r => r.GetCartByUserIdAsync("user1")).ReturnsAsync(cart);

        var request = new AddToCartRequest { ProductId = 1, Quantity = 3 };

        // Act
        await _service.AddToCartAsync("user1", request);

        // Assert: quantity tăng từ 2 lên 5, UpdateAsync được gọi
        Assert.Equal(5, existingItem.Quantity);
        _mockOrderRepo.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
    }

    // --- UpdateItemQuantityAsync ---

    [Fact]
    public async Task UpdateItemQuantityAsync_WhenQuantityIsZero_ShouldThrowValidationException()
    {
        // Arrange: quantity = 0, vi phạm rule quantity > 0
        var act = async () => await _service.UpdateItemQuantityAsync("user1", 1, 0);

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_WhenCartNotFound_ShouldThrowNotFoundException()
    {
        // Arrange: user không có cart
        _mockOrderRepo.Setup(r => r.GetCartByUserIdAsync("user1")).ReturnsAsync((Order?)null);

        var act = async () => await _service.UpdateItemQuantityAsync("user1", 1, 5);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    // --- CheckoutCart ---

    [Fact]
    public async Task CheckoutCart_WhenCartIsEmpty_ShouldThrowValidationException()
    {
        // Arrange: cart tồn tại nhưng không có item nào
        var cart = new Order
        {
            OrderID = 10,
            UserID = "user1",
            Status = "New",
            OrderDetails = new List<OrderDetail>()
        };
        _mockOrderRepo.Setup(r => r.GetCartByUserIdAsync("user1")).ReturnsAsync(cart);

        var act = async () => await _service.CheckoutCart("user1");

        // Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    [Fact]
    public async Task CheckoutCart_WhenCartHasItems_ShouldCalculateTotalAndSetStatusCheckedOut()
    {
        // Arrange: cart có 2 items
        var cart = new Order
        {
            OrderID = 10,
            UserID = "user1",
            Status = "New",
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail { UnitPrice = 100, Quantity = 2 }, // 200
                new OrderDetail { UnitPrice = 50,  Quantity = 3 }  // 150
            }
        };
        _mockOrderRepo.Setup(r => r.GetCartByUserIdAsync("user1")).ReturnsAsync(cart);
        _mockOrderRepo.Setup(r => r.UpdateAsync(It.IsAny<Order>())).ReturnsAsync(cart);

        // Act
        await _service.CheckoutCart("user1");

        // Assert: TotalAmount = 350, status = "Checked Out"
        _mockOrderRepo.Verify(r => r.UpdateAsync(It.Is<Order>(o =>
            o.TotalAmount == 350 &&
            o.Status == "Checked Out"
        )), Times.Once);
    }
}
