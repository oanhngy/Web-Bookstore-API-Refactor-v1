using BookstoreWeb.Application.DTOs.Cart;

public interface ICartService
{
    //return current cart, return empty of not found
    Task<CartResponse> GetCartAsync(string userId);

    //add product to cart, tạo new cart nếu chưa có
    Task AddToCartAsync(string userId, AddToCartRequest request);

    //remove item
    Task RemoveFromCartAsync(string userId, int orderDetailId);

    //update quantity of an item
    Task UpdateItemQuantityAsync(string userId, int orderDetailId, int quantity);

    //change cart status new-> update, return updated cart
    Task<CartResponse> CheckoutCart(string userId);

}