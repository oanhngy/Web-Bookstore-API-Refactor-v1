using BookstoreWeb.Application.Domain;
namespace BookstoreWeb.Application.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int orderId);
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
    Task<Order?> GetCartByUserIdAsync(string userId); //order w/ status=new
    Task<Order> AddAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<IEnumerable<Order>> GetCompletedOrderAsync(); //cho revenue data
}