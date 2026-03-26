namespace BookstoreWeb.Application.DTOs.Orders;

//full order
public class OrderResponse
{
    public int OrderId {get; set;}
    public string Status {get; set;}=string.Empty;
    public DateTime? OrderDate {get; set;}
    public decimal? TotalAmount {get; set;}
    public string? FullName {get; set;}
    public string? Email {get; set;}
    public string? Phone {get; set;}
    public string? Address {get; set;}
    public string? Note {get; set;}
    public string? PaymentMethod {get; set;}
    public List<OrderDetailResponse> OrderDetails {get; set;}=new();
}