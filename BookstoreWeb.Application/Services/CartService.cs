using System.Security.Cryptography.X509Certificates;
using BookstoreWeb.Application.DTOs.Cart;
using BookstoreWeb.Application.Exceptions;
using BookstoreWeb.Application.Interfaces;
using BookstoreWeb.Infrastructure.Data;
using Microsoft.Extensions.Logging;
namespace BookstoreWeb.Application.Services;

public class CartService : ICartService
{
    //cần 2 repo: IOrder=đọc/ghi cart (Order), IProduct=lấy sp add to cart
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    private readonly ILogger<CartService> _logger;

    //contructor nhận 3 dependency: Product Repo -> OrderRepo -> Cart Service
    public CartService(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<CartService> logger)
    {
        _orderRepository=orderRepository;
        _productRepository=productRepository;
        _logger=logger;
    }

    //1-get cart hiện tại: k throw nếu chưa có
    public async Task<CartResponse> GetCartAsync(string userId)
    {
        _logger.LogInformation("Retrieving cart for user {UserId}", userId);
        var cart=await _orderRepository.GetCartByUserIdAsync(userId);

        //k throw
        if(cart==null) return new CartResponse();
        return ToCartResponse(cart);
    }

    //2- add to cart
    public async Task AddToCartAsync(string userId, AddToCartRequest request)
    {
        _logger.LogInformation("Adding product {ProductId} to cart for user {UserId}", request.ProductId, userId);
        //check product exist k
        var product=await _productRepository.GetByIdAsync(request.ProductId);
        if(product==null)
        {
            throw new NotFoundException($"Product with id {request.ProductId} was not found");
        }

        //tìm cart, Cart có status=new, nulll(chưa có cart) -> tạo
        var cart=await _orderRepository.GetCartByUserIdAsync(userId);
        if(cart==null)
        {
            cart=new Order
            {
                UserID=userId,
                Status="New",
                OrderDetails=new List<OrderDetail>() //tạo list rỗng
            };
            //thêm 1st item vào cart mới tạo
            cart.OrderDetails.Add(new OrderDetail
            {
                ProductID=product.ProductID,
                Quantity=request.Quantity,
                UnitPrice=product.Price //giá ở thời điểm đặt
            });
            //bỏ vô db
            await _orderRepository.AddAsync(cart);
        }
        else //đã có cart, check OrderDetails k null
        {
            cart.OrderDetails??=new List<OrderDetail>(); //??= nghĩa là nếu null -> gán list rỗng, !null -> skip
            var existingItem=cart.OrderDetails.FirstOrDefault(d=>d.ProductID==request.ProductId);

            if(existingItem!=null)
            {
                existingItem.Quantity+=request.Quantity; //cộng thêm
            }
            else
            {
                cart.OrderDetails.Add(new OrderDetail
                {
                    ProductID=product.ProductID,
                    Quantity=request.Quantity,
                    UnitPrice=product.Price
                });
            }
            await _orderRepository.UpdateAsync(cart);
        }
    }

    //3-remove-xóa 1 item khỏi cart
    //xóa theo orderDetailId
    public async Task RemoveFromCartAsync(string userId, int orderDetailId)
    {
        _logger.LogInformation("Removing item {OrderDetailId} from cart for user {UserId}", orderDetailId, userId);
        //lấy cart của user
        var cart=await _orderRepository.GetCartByUserIdAsync(userId);
        if(cart==null) throw new NotFoundException("Cart not found");

        //tim item cần xóa trong cart.OrderDetails (k query db riêng)
        var item=cart.OrderDetails?.FirstOrDefault(d=>d.OrderDetailID==orderDetailId);
        if(item==null) throw new NotFoundException($"Cart item with id {orderDetailId} was not found");
        cart.OrderDetails!.Remove(item);

        //save lại
        await _orderRepository.UpdateAsync(cart);
    }

    //4-update quantity 1 item
    public async Task UpdateItemQuantityAsync(string userId, int orderDetailId, int quantity)
    {
        _logger.LogInformation("Updating quantity of item {OrderDetailId} to {Quantity} for user {USerId}", orderDetailId, quantity, userId);
        
        //validate trước khi vào db, make sure quantity>0
        if(quantity<=0) throw new ValidationException("Quantity must be greater than 0");

        //lấy cart user
        var cart=await _orderRepository.GetCartByUserIdAsync(userId);
        if(cart==null) throw new NotFoundException("Cart not found");

        //tìm item cần update
        var item=cart.OrderDetails?.FirstOrDefault(d=>d.OrderDetailID==orderDetailId);
        if(item==null) throw new NotFoundException($"Cart item with id {orderDetailId} was not found");

        item.Quantity=quantity; //gán quatity mới=ghi đè
        await _orderRepository.UpdateAsync(cart);
    }

        //5-checkout-update status new -> checked out
        public async Task<CartResponse> CheckoutCart(string userId)
        {
            _logger.LogInformation("Checking out cart for user {UserId}", userId);
            var cart=await _orderRepository.GetCartByUserIdAsync(userId);
            if(cart==null) throw new NotFoundException("Cart not found");

            //k checkout nếu cart empty
            if(cart.OrderDetails==null || !cart.OrderDetails.Any()) throw new ValidationException("Cannot checkout an empty cart");

            //tính tổng tiền mỗi item
            cart.TotalAmount=cart.OrderDetails.Sum(d=>d.UnitPrice*d.Quantity);
            cart.Status="Checked Out";
            cart.OrderDate=DateTime.UtcNow;
            
            var updated=await _orderRepository.UpdateAsync(cart);
            _logger.LogInformation("cart checked out successfully for user {USerId}", userId);
            
            return ToCartResponse(updated);
        }

        //helper: map Order entity -> CartResponse DTO vì vs user đây là cart, not order. CartResponse chứa đúng field user cần trong giò hàng
        private static CartResponse ToCartResponse(Order cart)
    {
        return new CartResponse
        {
            OrderId=cart.OrderID,
            Items=cart.OrderDetails?.Select(d=>new CartItemResponse
            {
                OrderDetailId=d.OrderDetailID,
                ProductId=d.ProductID,
                ProductName=d.Product?.Name ?? string.Empty,
                UnitPrice=d.UnitPrice,
                Quantity=d.Quantity
            }).ToList() ?? new List<CartItemResponse>(),
            TotalAmount=cart.OrderDetails?.Sum(d=>d.UnitPrice*d.Quantity) ?? 0
        };
    }   
}