namespace BookstoreWeb.Application.DTOs.Orders;

//1 single line item
public class OrderDetailResponse
{
    public int OrderDetailId {get; set;}
    public int ProductId {get; set;}
    public string ProductName {get; set;}=string.Empty;
    public decimal UnitPrice {get; set;}
    public int Quantity {get; set;}
    public decimal Subtotal => UnitPrice*Quantity;
}