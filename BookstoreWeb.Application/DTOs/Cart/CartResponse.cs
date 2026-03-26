namespace BookstoreWeb.Application.DTOs.Cart;

public class CartResponse
{
    public int OrderId {get; set;}
    public List<CartItemResponse> Items {get; set;}=new();
    public decimal TotalAmount {get; set;}
}