using BookstoreWeb.Application.DTOs.Orders;
namespace BookstoreWeb.Application.Interfaces;

public interface IOrderService
{
    //return all orders of 1 user
    Task<IEnumerable<OrderResponse>> GetByUserIdAsync(string userId);

    //return a single order by id, throw NotFoundException
    Task<OrderResponse> GetByIdAsync(int orderId);

    //cancel order, throw NotFoundException
    Task CancelAsync(int orderId, string userId);

    //confirm a checkout order, return updated order
    Task<OrderResponse> ConfirmAsync(int orderId, ConfirmOrderRequest request, string userId);
}