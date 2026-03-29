using BookstoreWeb.Application.DTOs.Orders;
namespace BookstoreWeb.Application.Interfaces;

public interface IAdminOrderService
{
    //return all orders in sys, newest 1st
    Task<IEnumerable<OrderResponse>> GetAllAsync();

    //return async single order by id, throw NotFoundExp
    Task<OrderResponse> GetByIdAsync(int orderId);

    //update status
    Task UpdateStatusAsync(int orderId, UpdateOrderStatusRequest request);

    //return revenuw group by timeframe
    Task<IEnumerable<RevenueDataResponse>> GetRevenueDatasAsync(string timeFrame);
}