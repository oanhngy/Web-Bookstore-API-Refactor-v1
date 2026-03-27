using BookstoreWeb.Application.DTOs.Orders;
using BookstoreWeb.Application.Exceptions;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Infrastructure.Data;
using Microsoft.Extensions.Logging;
namespace BookstoreWeb.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
    {
        _orderRepository=orderRepository;
        _logger=logger;
    }

    //1-get by use id
    public async Task<IEnumerable<OrderResponse>> GetByUserIdAsync(string userId)
    {
        _logger.LogInformation("Retrieving orders for user {UserId}", userId);
        var orders=await _orderRepository.GetByUserIdAsync(userId);
        return orders.Select(ToResponse);
    }    

    //2-get by id (orderId)
    public async Task<OrderResponse> GetByIdAsync(int orderId)
    {
        _logger.LogInformation("Retrieving order with id {OrderId}", orderId);
        var order=await _orderRepository.GetByIdAsync(orderId);
        if(order==null)
        {
            _logger.LogWarning("Order with id {OrderId} was not found",orderId);
            throw new NotFoundException($"Order with id {orderId} was not found");
        }
        return ToResponse(order);
    }

    //3-cancel
    public async Task CancelAsync(int orderId, string userId)
    {
        _logger.LogInformation("Cancelling order {OrderId} for user {UserId}", orderId,userId);
        var order=await _orderRepository.GetByIdAsync(orderId);

        //security pattern: order null hoặc k đúng id -> đều throw NotFound
        //technic=security through obscurity cho resource ownership
        if(order==null || order.UserID!=userId)
        {
            _logger.LogWarning("Order {OrderId} not found for user {UserId}", orderId, userId);
            throw new NotFoundException($"Order with id {orderId} was not found");
        }

        //business rule
        //tạo array string inline thay vì if/else -> dễ mở rộng
        //chỉ thêm array, k sửa logic
        var cancellableStatuses=new[] {"Checked Out", "Confirmed"};
        if(!cancellableStatuses.Contains(order.Status)) throw new ValidationException($"Order with status '{order.Status}' can not be cancelled");

        order.Status = "Cancelled";
        await _orderRepository.UpdateAsync(order);
        _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);
    }

    //4-confirm, sau checkout, user điền info để đặt hàng, sau đó Admin chuyển thành confirmed
    public async Task<OrderResponse> ConfirmAsync(int orderId, ConfirmOrderRequest request, string userId)
    {
        _logger.LogInformation("Confirming order {OrderId} for user {UserId}", orderId, userId);
        var order=await _orderRepository.GetByIdAsync(orderId);

        //security check, giống 3
        if (order == null || order.UserID != userId)
        {
            _logger.LogWarning("Order {OrderId} not found for user {UserId}", orderId, userId);
            throw new NotFoundException($"Order with id {orderId} was not found");
        }

        //business rule
        //xài != thay cho .Contains() vì chỉ có 1 status hợp lệ
        if(order.Status!="Checked Out") throw new ValidationException(
                $"Only orders with status 'Checked Out' can be confirmed. " +
                $"Current status: '{order.Status}'");
        
        //map info giao hàng DTO request -> entity, only field đc phép not all
        order.FullName=request.FullName;
        order.Email=request.Email;
        order.Phone=request.Phone;
        order.Address=request.Address;
        order.Note=request.Note;
        order.PaymentMethod=request.PaymentMethod;

        order.Status="Confirmed";
        var updated=await _orderRepository.UpdateAsync(order);
        _logger.LogInformation("Order {OrderId} confirmed successfully", orderId);
        return ToResponse(updated);
    }

    //helper
    private static OrderResponse ToResponse(Order order)
    {
        return new OrderResponse
        {
            OrderId = order.OrderID,
            Status = order.Status,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            FullName = order.FullName,
            Email = order.Email,
            Phone = order.Phone,
            Address = order.Address,
            Note = order.Note,
            PaymentMethod = order.PaymentMethod,
            OrderDetails = order.OrderDetails?.Select(d => new OrderDetailResponse
            {
                OrderDetailId = d.OrderDetailID,
                ProductId = d.ProductID,
                ProductName = d.Product?.Name ?? string.Empty,
                UnitPrice = d.UnitPrice,
                Quantity = d.Quantity
            }).ToList() ?? new List<OrderDetailResponse>()
        };
    }

    
}