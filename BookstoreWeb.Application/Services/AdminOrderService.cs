using BookstoreWeb.Application.DTOs.Orders;
using BookstoreWeb.Application.Exceptions;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Infrastructure.Data;
using Microsoft.Extensions.Logging;
namespace BookstoreWeb.Application.Services;

public class AdminOrderService : IAdminOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<AdminOrderService> _logger;
    public AdminOrderService(IOrderRepository orderRepository, ILogger<AdminOrderService> logger)
    {
        _orderRepository=orderRepository;
        _logger=logger;
    }

    //1-get all
    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        _logger.LogInformation("Admin retrieving all orders");
        var orders=await _orderRepository.GetAllAsync();
        return orders.Select(ToResponse);
    }

    //2-get by orderId
    public async Task<OrderResponse> GetByIdAsync(int orderId)
    {
        _logger.LogInformation("Admin retrieving order with id {OrderId}",orderId);
        var order=await _orderRepository.GetByIdAsync(orderId);
        if(order==null)
        {
            _logger.LogWarning("Order with {OrderId} was not found", orderId);
            throw new NotFoundException($"Order with id {orderId} was not found");
        }
        return ToResponse(order);
    }

    //3-u[date status
    public async Task UpdateStatusAsync(int orderId, UpdateOrderStatusRequest request)
    {
        _logger.LogInformation("Admin updating status of order {OrderId} to {Status}",orderId,request.Status);
        var order= await _orderRepository.GetByIdAsync(orderId);
        if(order==null)
        {
            _logger.LogWarning("Order with id {OrderId} was not found", orderId);
            throw new NotFoundException($"Order with id {orderId} was not found");
        }
        var validStatuses=new[] {"Confirmed", "Completed", "Cancelled"};
        if(!validStatuses.Contains(request.Status)) throw new ValidationException(
                $"Invalid status '{request.Status}'. " +
                $"Valid values: {string.Join(", ", validStatuses)}");

        order.Status=request.Status;
        await _orderRepository.UpdateAsync(order);
        _logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, request.Status);
    }

    //4-get venue report theo timeFrame
    public async Task<IEnumerable<RevenueDataResponse>> GetRevenueDatasAsync(string timeFrame)
    {
        _logger.LogInformation("Admin retrieving revenue data with timeFrame {TimeFrame}", timeFrame);
        var completedOrders=await _orderRepository.GetCompletedOrderAsync();

        var revenueData=timeFrame.ToLower() switch
        {
            "daily"=>completedOrders
                .Where(o=>o.OrderDate.HasValue)
                .GroupBy(o=>o.OrderDate!.Value.ToString("yyyy-MM-dd"))
                .Select(g=>new RevenueDataResponse
                {
                    Label=g.
                    Key,
                    TotalRevenue=g.Sum(o=>o.TotalAmount??0)
                }),

            "monthly"=>completedOrders
                .Where(o=>o.OrderDate.HasValue)
                .GroupBy(o=>o.OrderDate!.Value.ToString("yyyy-MM"))
                .Select(g=>new RevenueDataResponse
                {
                    Label=g.
                    Key,
                    TotalRevenue=g.Sum(o=>o.TotalAmount??0)
                }),

            "yearly"=>completedOrders
                .Where(o=>o.OrderDate.HasValue)
                .GroupBy(o=>o.OrderDate!.Value.ToString("yyyy"))
                .Select(g=>new RevenueDataResponse
                {
                    Label=g.
                    Key,
                    TotalRevenue=g.Sum(o=>o.TotalAmount??0)
                }),

            //timeFrame k hợp lệ
            _=> throw new ValidationException($"Invalid timeFrame '{timeFrame}'. Valid values: daily, monthly, yearly")
        };
        return revenueData.OrderBy(r=>r.Label);
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