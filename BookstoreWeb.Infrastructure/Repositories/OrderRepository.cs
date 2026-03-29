using BookstoreWeb.Application.Domain;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace BookstoreWeb.Infrastructure.Repositories;

//vs Orders và OrderDetails
public class OrderRepository:IOrderRepository
{
    private readonly BookstoreContext _context;
    public OrderRepository(BookstoreContext context)
    {
        _context=context;
    }

    //1-get all, kèm detail-cho admin
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o=>o.OrderDetails)
                .ThenInclude(od=>od.Product)
            .OrderByDescending(o=>o.OrderDate)
            .ToListAsync();
    }

    //2-get order by id
    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o=>o.OrderDetails)
                .ThenInclude(od=>od.Product)
            .FirstOrDefaultAsync(o=>o.OrderID==orderId);
    }

    //3-lấy all order của 1 user (trừ giỏ status=new)
    public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Include(o=>o.OrderDetails)
                .ThenInclude(od=>od.Product)
            .Where(o=>o.UserID==userId && o.Status!="New")
            .OrderByDescending(o=>o.OrderDate)
            .ToListAsync();
    }

    //4-lấy cart hiện tại của user(='New"), chỉ 1 tại 1 time
    public async Task<Order?> GetCartByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Include(o=>o.OrderDetails)
                .ThenInclude(od=>od.Product)
            .FirstOrDefaultAsync(o=>o.UserID==userId && o.Status=="New");
    }

    //4-add mới
    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    //5-update
    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    //6-lấy all đơn status=Completed, để tính venue theo time
    public async Task<IEnumerable<Order>> GetCompletedOrderAsync()
    {
        return await _context.Orders
            .Include(o=>o.OrderDetails)
            .Where(o=>o.Status=="Completed")
            .ToListAsync();
    }
}